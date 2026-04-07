using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Linq;

namespace SafeDoc
{
    public static class GestorRed
    {
        private const int PuertoBase = 5555;

        // Convierte IP a un código "SafeDoc"
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
            while (base64.Length % 4 != 0) base64 += "=";
            byte[] ipBytes = Convert.FromBase64String(base64);
            return new IPAddress(ipBytes).ToString();
        }

        public static string ObtenerIPLocal()
        {
            // Intentamos buscar la IP en adaptadores Reales (WiFi/Ethernet)
            // Evitamos adaptadores de VMware, VirtualBox, Docker o WSL
            var interfaces = NetworkInterface.GetAllNetworkInterfaces()
                .Where(i => i.OperationalStatus == OperationalStatus.Up)
                .Where(i => i.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || 
                           i.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                .Where(i => !i.Description.ToLower().Contains("virtual") && 
                           !i.Description.ToLower().Contains("pseudo") &&
                           !i.Description.ToLower().Contains("docker") &&
                           !i.Description.ToLower().Contains("wsl"));

            foreach (var ni in interfaces)
            {
                var props = ni.GetIPProperties();
                var ipv4 = props.UnicastAddresses.FirstOrDefault(a => a.Address.AddressFamily == AddressFamily.InterNetwork);
                if (ipv4 != null) return ipv4.Address.ToString();
            }

            // Fallback al método tradicional si falla el filtro
            var host = Dns.GetHostEntry(Dns.GetHostName());
            return host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)?.ToString() ?? "127.0.0.1";
        }

        // SERVIDOR: Envía una carpeta
        public static async Task EnviarCarpetaAsync(Carpeta carpeta, Action<string> log)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, PuertoBase);
            try
            {
                listener.Start();
                log($"SERVIDOR ACTIVO en IP: {ObtenerIPLocal()}");
                log("Esperando conexión de confianza (RECUERDA: MISMA RED WIFI)...");
                
                using TcpClient client = await listener.AcceptTcpClientAsync();
                log("¡CONECTADO! Empezando envío...");

                using NetworkStream stream = client.GetStream();
                
                string json = JsonSerializer.Serialize(carpeta);
                byte[] data = GestorArchivos.CifrarManual(json, "P2P_TRANSFER");

                // Enviamos tamaño (4 bytes)
                byte[] sizeBytes = BitConverter.GetBytes(data.Length);
                await stream.WriteAsync(sizeBytes, 0, 4);
                
                // Enviamos contenido
                await stream.WriteAsync(data, 0, data.Length);
                log("¡Carpeta enviada correctamente!");
            }
            catch (Exception ex)
            {
                log("Error en Servidor: " + ex.Message);
                throw;
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
                log($"Intentando conectar a {ip} (Puerto {PuertoBase})...");
                
                // Timeout de 5 segundos para no esperar "mucho" si el firewall bloquea
                var connectTask = client.ConnectAsync(ip, PuertoBase);
                if (await Task.WhenAny(connectTask, Task.Delay(5000)) != connectTask)
                {
                    log("TIEMPO AGOTADO: El otro PC no responde. ¿Estáis en la misma red?");
                    return null;
                }

                using NetworkStream stream = client.GetStream();
                log("¡CONECTADO! Descargando datos...");

                // Leemos tamaño (asegurando 4 bytes)
                byte[] sizeBuffer = new byte[4];
                int totalRead = 0;
                while (totalRead < 4)
                {
                    int r = await stream.ReadAsync(sizeBuffer, totalRead, 4 - totalRead);
                    if (r == 0) throw new Exception("Conexión cerrada prematuramente.");
                    totalRead += r;
                }
                int size = BitConverter.ToInt32(sizeBuffer, 0);
                log($"Tamaño detectado: {size} bytes. Descargando...");

                // Leemos datos en fragmentos
                byte[] data = new byte[size];
                int read = 0;
                while (read < size)
                {
                    int r = await stream.ReadAsync(data, read, size - read);
                    if (r == 0) break;
                    read += r;
                }

                log("Desencriptando y procesando carpeta...");
                string json = GestorArchivos.DescifrarManual(data, "P2P_TRANSFER");
                return JsonSerializer.Deserialize<Carpeta>(json);
            }
            catch (SocketException sex)
            {
                log("ERROR DE RED: El firewall o el router está bloqueando la conexión.");
                return null;
            }
            catch (Exception ex)
            {
                log("ERROR: " + ex.Message);
                return null;
            }
        }
    }
}
