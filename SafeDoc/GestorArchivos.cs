using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace SafeDoc
{
    public static class GestorArchivos
    {
        private static readonly string ruta = "datos.dat";

        public static BaseDeDatos LeerBase()
        {
            try
            {
                if (!File.Exists(ruta)) return new BaseDeDatos();
                string json = File.ReadAllText(ruta);
                if (string.IsNullOrWhiteSpace(json)) return new BaseDeDatos();
                var b = JsonSerializer.Deserialize<BaseDeDatos>(json);
                return b ?? new BaseDeDatos();
            }
            catch
            {
                // Si el archivo está corrupto o es de una versión vieja, empezamos de cero
                return new BaseDeDatos();
            }
        }

        private static void GuardarBase(BaseDeDatos db)
        {
            string json = JsonSerializer.Serialize(db);
            File.WriteAllText(ruta, json);
        }

        public static bool CrearUsuario(string username, string password)
        {
            var db = LeerBase();
            if (db.Usuarios.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
                return false; 

            Carpeta raizVacia = new Carpeta("Raiz");
            var user = new UsuarioRecord
            {
                Username = username,
                PasswordHash = HashPassword(password),
                CarpetaCifrada = CifrarCarpeta(raizVacia, password)
            };
            
            db.Usuarios.Add(user);
            GuardarBase(db);
            return true;
        }

        public static Carpeta CargarCarpetaUsuario(string username, string password)
        {
            var db = LeerBase();
            var user = db.Usuarios.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            if (user == null || user.PasswordHash != HashPassword(password))
                throw new Exception("Credenciales incorrectas");

            if (user.CarpetaCifrada == null || user.CarpetaCifrada.Length == 0)
                return new Carpeta("Raiz");

            string json = Descifrar(user.CarpetaCifrada, password);
            var raiz = JsonSerializer.Deserialize<Carpeta>(json);
            
            if (raiz == null) throw new Exception("Error al decodificar");
            AsignarCarpetaMadre(raiz);
            return raiz;
        }

        public static void GuardarCarpetaUsuario(string username, string password, Carpeta raiz)
        {
            var db = LeerBase();
            var user = db.Usuarios.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            if (user != null)
            {
                user.CarpetaCifrada = CifrarCarpeta(raiz, password);
                GuardarBase(db);
            }
        }

        public static bool ValidarLogin(string username, string password)
        {
            var db = LeerBase();
            var user = db.Usuarios.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            return user != null && user.PasswordHash == HashPassword(password);
        }

        private static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        private static void AsignarCarpetaMadre(Carpeta carpeta)
        {
            foreach (var elemento in carpeta.Contenido)
            {
                elemento.CarpetaMadre = carpeta;
                if (elemento is Carpeta subCarpeta)
                    AsignarCarpetaMadre(subCarpeta);
            }
        }

        private static byte[] CifrarCarpeta(Carpeta raiz, string password)
        {
            string json = JsonSerializer.Serialize(raiz);
            return Cifrar(json, password);
        }

        private static byte[] Cifrar(string texto, string password)
        {
            using Aes aes = Aes.Create();
            byte[] salt = Encoding.UTF8.GetBytes("SAFE_DOC_SALT");
            var key = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
            aes.Key = key.GetBytes(32);
            aes.IV = key.GetBytes(16);

            using MemoryStream ms = new();
            using (CryptoStream cs = new(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            using (StreamWriter sw = new(cs))
            {
                sw.Write(texto);
            }
            return ms.ToArray();
        }

        public static string CompartirCarpeta(Carpeta carpeta)
        {
            var db = LeerBase();
            string codigo = Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
            
            // Ciframos los datos de la carpeta usando el código como "password"
            byte[] cifrado = CifrarCarpeta(carpeta, codigo);
            
            db.Transferencias.Add(new TransferenciaRecord { 
                Codigo = codigo, 
                ContenidoCifrado = cifrado 
            });
            
            GuardarBase(db);
            return codigo;
        }

        public static Carpeta ImportarCarpeta(string codigo)
        {
            var db = LeerBase();
            var trans = db.Transferencias.FirstOrDefault(t => t.Codigo == codigo.ToUpper());
            
            if (trans == null) throw new Exception("Código no válido.");
            
            string json = Descifrar(trans.ContenidoCifrado, codigo.ToUpper());
            var carpeta = JsonSerializer.Deserialize<Carpeta>(json);
            
            if (carpeta == null) throw new Exception("Error al procesar los datos.");
            
            // Opcional: Eliminar la transferencia tras el primer uso para mayor seguridad
            // db.Transferencias.Remove(trans);
            // GuardarBase(db);
            
            AsignarCarpetaMadre(carpeta);
            return carpeta;
        }

        private static string Descifrar(byte[] datos, string password)
        {
            using Aes aes = Aes.Create();
            byte[] salt = Encoding.UTF8.GetBytes("SAFE_DOC_SALT");
            var key = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
            aes.Key = key.GetBytes(32);
            aes.IV = key.GetBytes(16);

            using MemoryStream ms = new(datos);
            using CryptoStream cs = new(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using StreamReader sr = new(cs);
            return sr.ReadToEnd();
        }
    }
}