using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace TestJson
{
    [JsonDerivedType(typeof(Carpeta), "Carpeta")]
    [JsonDerivedType(typeof(Archivo), "Archivo")]
    public abstract class Elemento {
        public string Nombre { get; set; } = string.Empty;
        [JsonIgnore] public Carpeta? CarpetaMadre { get; set; }
    }
    public class Carpeta : Elemento {
        public Carpeta() { }
        public Carpeta(string n, Carpeta? c = null) { Nombre=n; CarpetaMadre=c; }
        public List<Elemento> Contenido { get; set; } = new List<Elemento>();
    }
    public class Archivo : Elemento {
        public Archivo() { }
        public Archivo(string n, Carpeta? c = null) { Nombre=n; CarpetaMadre=c; }
    }
    public class DatosSeguros {
        public string Verificacion { get; set; } = "SAFE_DOC_OK";
        public Carpeta Raiz { get; set; } = new Carpeta();
    }

    class Program
    {
        static void Main()
        {
            try
            {
                var raiz = new Carpeta("Raiz");
                raiz.Contenido.Add(new Carpeta("Mis Documentos", raiz));
                raiz.Contenido.Add(new Archivo("ClavesSecretas.txt", raiz));
                
                var datos = new DatosSeguros { Raiz = raiz };
                string json = JsonSerializer.Serialize(datos);
                Console.WriteLine("SERIALIZE OK. Length: " + json.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine("SERIALIZE FAIL: " + ex.ToString());
            }
        }
    }
}
