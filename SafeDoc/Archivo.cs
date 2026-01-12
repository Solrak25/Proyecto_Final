using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeDoc
{
    public class Archivo
    {
        public string Nombre { get; set; }
        public Carpeta CarpetaMadre { get; set; }

        public Archivo(string nombre, Carpeta carpetaMadre)
        {
            Nombre = nombre;
            CarpetaMadre = carpetaMadre;
        }
    }
}
