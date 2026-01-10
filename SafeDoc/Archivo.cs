using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeDoc
{
    public class Archivo
    {
        public Carpeta CarpetaPadre { get; set; }
        public string Nombre { get; set; }
        public string Ruta { get; set; }

        public Archivo(String nombre, Carpeta carpetaPadre, string ruta) { 
            this.Nombre = nombre;
            this.Ruta = ruta;
            this.CarpetaPadre = carpetaPadre;
        }

        public Archivo(String nombre)
        {
            this.Nombre = nombre;
        }
    }
}
