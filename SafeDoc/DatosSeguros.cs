using System.Collections.Generic;

namespace SafeDoc
{
    public class BaseDeDatos
    {
        public string Verificacion { get; set; } = "SAFE_DOC_MULTIPROFILE";
        public List<UsuarioRecord> Usuarios { get; set; } = new List<UsuarioRecord>();
        public List<TransferenciaRecord> Transferencias { get; set; } = new List<TransferenciaRecord>();
    }

    public class UsuarioRecord
    {
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty; // SHA256 base64
        public byte[] CarpetaCifrada { get; set; } = null!;
    }

    public class TransferenciaRecord
    {
        public string Codigo { get; set; } = string.Empty;
        public byte[] ContenidoCifrado { get; set; } = null!;
    }
}
