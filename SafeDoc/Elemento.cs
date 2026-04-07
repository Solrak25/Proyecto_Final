using SafeDoc;
using System.Text.Json.Serialization;

namespace SafeDoc
{

    [JsonDerivedType(typeof(Carpeta), "Carpeta")]
    [JsonDerivedType(typeof(Archivo), "Archivo")]
    public abstract class Elemento
    {
        public string Nombre { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonIgnore]
        public Carpeta? CarpetaMadre { get; set; }
    }
}