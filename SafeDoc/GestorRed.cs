using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace SafeDoc
{
    public static class GestorRed
    {
        private const int PuertoBase = 5555;

        // Convierte IP y Puerto a un código "SafeDoc"
        public static string GenerarCodigo(string ip)
        {
            byte[] ipBytes = IPAddress.Parse(ip).GetAddressBytes();
            string code = Convert.ToBase64String(ipBytes).Replace("=", "");
            return $"SAFE-{code}";
        }

        // Recupera la IP desde el código
        public static string ObtenerIPDesdeCodigo(string codigo)
        {
            if (!codigo.StartsWith("SAFE-")) throw new Exception("Código no válido.");
            string base64 = codigo.Replace("SAFE-", "");
            
            // Re-añadir el padding para Base64 si es necesario
            while (base64.Length % 4 != 0) base64 += "=";
            
            byte[] ipBytes = Convert.FromBase64String(base64);
            return new IPAddress(ipBytes).ToString();
        }

        public static string ObtenerIPLocal()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "127.0.0.1";
        }

        // SERVIDOR: Envía una carpeta
        public static async Task EnviarCarpetaAsync(Carpeta carpeta, Action<string> log)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, PuertoBase);
            try
            {
                listener.Start();
                log("Esperando conexión...");
                
                using TcpClient client = await listener.AcceptTcpClientAsync();
                log("Cliente conectado. Enviando datos...");

                using NetworkStream stream = client.GetStream();
                
                // Serializamos y ciframos (usando el puerto como password temporal o nada si ya confían en la red)
                // Para simplificar y mantener el esquema, ciframos con "P2P_TRANSFER"
                string json = JsonSerializer.Serialize(carpeta);
                byte[] data = GestorArchivos.CifrarManual(json, "P2P_TRANSFER");

                // Enviamos tamaño primero
                byte[] size = BitConverter.GetBytes(data.Length);
                await stream.WriteAsync(size, 0, size.Length);
                
                // Enviamos contenido
                await stream.WriteAsync(data, 0, data.Length);
                log("¡Transferencia completada!");
            }
            finally
            {
                listener.Stop();
            }
        }

        // CLIENTE: Recibe una carpeta
        public static async Task<Carpeta?> RecibirCarpetaAsync(string ip, Action<string> log)
        {
            try
            {
                using TcpClient client = new TcpClient();
                log($"Conectando a {ip}...");
                await client.ConnectAsync(ip, PuertoBase);
                
                using NetworkStream stream = client.GetStream();
                log("Conectado. Descargando...");

                // Leemos tamaño
                byte[] sizeBuffer = new byte[4];
                await stream.ReadAsync(sizeBuffer, 0, 4);
                int size = BitConverter.GetBytes(BitConverter.ToUInt32(sizeBuffer, 0)).Length == 4 
                           ? BitConverter.ToInt32(sizeBuffer, 0) : 0;

                // Leemos datos
                byte[] data = new byte[size];
                int read = 0;
                while (read < size)
                {
                    int r = await stream.ReadAsync(data, read, size - read);
                    if (r == 0) break;
                    read += r;
                }

                log("Descifra y procesando...");
                string json = GestorArchivos.DescifrarManual(data, "P2P_TRANSFER");
                return JsonSerializer.Deserialize<Carpeta>(json);
            }
            catch (Exception ex)
            {
                log("Error: " + ex.Message);
                return null;
            }
        }
    }
}
