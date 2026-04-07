using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeDoc
{
    public class Archivo : Elemento
    {
        public string ContenidoTexto { get; set; } = string.Empty;

        public Archivo() { }

        public Archivo(string nombre, Carpeta? carpetaMadre)
        {
            Nombre = nombre;
            CarpetaMadre = carpetaMadre;
        }
    }
}
