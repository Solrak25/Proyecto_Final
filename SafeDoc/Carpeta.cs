using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeDoc
{
    public class Carpeta : Elemento
    {
        public Carpeta() { }

        public Carpeta(string nombre, Carpeta? carpetaMadre = null)
        {
            Nombre = nombre;
            CarpetaMadre = carpetaMadre;
        }

        public List<Elemento> Contenido { get; set; } = new();
    }
}