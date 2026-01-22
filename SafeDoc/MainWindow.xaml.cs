using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;


namespace SafeDoc
{
    public partial class MainWindow : Window
    {
        public List<object> listaElementos = new List<object>();

        private object elementoSeleccionado;
        private object clipboardElemento;
        private Carpeta carpetaActual;

        public MainWindow()
        {
            InitializeComponent();

            Carpeta carpetaRaiz = new Carpeta("Raiz");
            carpetaActual = carpetaRaiz;
            listaElementos.Add(carpetaRaiz);

            Archivo archivo1 = new Archivo("Archivo1.txt", carpetaRaiz);
            Archivo archivo2 = new Archivo("Archivo2.txt", carpetaRaiz);
            Archivo archivo3 = new Archivo("Archivo3.txt", carpetaRaiz);
            Archivo archivo4 = new Archivo("Archivo4.txt", carpetaRaiz);
            Archivo archivo5 = new Archivo("Archivo5.txt", carpetaRaiz);
            Carpeta carpeta1 = new Carpeta("Carpeta1", carpetaRaiz);
            Archivo archivo6 = new Archivo("Archivo6.txt", carpeta1);
            Carpeta carpeta2 = new Carpeta("Carpeta2", carpeta1);
            Archivo archivo7 = new Archivo("Archivo7.txt", carpeta2);

            listaElementos.Add(archivo1);
            listaElementos.Add(archivo2);
            listaElementos.Add(archivo3);
            listaElementos.Add(archivo4);
            listaElementos.Add(archivo5);
            listaElementos.Add(carpeta1);
            listaElementos.Add(archivo6);
            listaElementos.Add(carpeta2);
            listaElementos.Add(archivo7);

            abrir(carpetaRaiz);
        }

        private void Copy_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (elementoSeleccionado == null) return;
            clipboardElemento = elementoSeleccionado;
        }

        private void Cut_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (elementoSeleccionado == null) return;

            clipboardElemento = elementoSeleccionado;
            eliminarElemento(elementoSeleccionado);
        }

        private void Paste_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (clipboardElemento == null) return;

            // Si era Archivo
            if (clipboardElemento is Archivo a)
                añadirElemento(new Archivo(a.Nombre, carpetaActual));

            // Si era Carpeta
            else if (clipboardElemento is Carpeta c)
                añadirElemento(new Carpeta(c.Nombre));
        }

        public void añadirElemento(object elemento)
        {
            Button btn = new Button();
            btn.Background = Brushes.Transparent;
            btn.Tag = elemento;
            btn.Click += btnElemento_Click;

            // texto mostrado
            if (elemento is Archivo a)
            {
                btn.Content = comprobarNombre(a.Nombre);
                btn.Template = (ControlTemplate)FindResource("Archivos");
            }
            else if (elemento is Carpeta c)
            {
                btn.Content = comprobarNombre(c.Nombre);
                btn.Template = (ControlTemplate)FindResource("Carpetas");
                btn.MouseDoubleClick += (s, e) =>
                {
                    abrir(c);
                };
            }

            WPExplorador.Children.Add(btn);
        }

        public void eliminarElemento(object elemento)
        {
            Button botonAEliminar = null;

            foreach (UIElement ui in WPExplorador.Children)
            {
                if (ui is Button b && Equals(b.Tag, elemento))
                {
                    botonAEliminar = b;
                    break;
                }
            }

            if (botonAEliminar != null)
                WPExplorador.Children.Remove(botonAEliminar);
        }

        private void btnElemento_Click(object sender, RoutedEventArgs e)
        {
            foreach (Button item in WPExplorador.Children)
                item.Background = Brushes.Transparent;

            elementoSeleccionado = ((Button)sender).Tag;

            Button btnSeleccionado = sender as Button;
            btnSeleccionado.Background = Brushes.LightBlue;
        }

        public void abrir(Carpeta carpeta)
        {
            WPExplorador.Children.Clear();
            carpetaActual = carpeta;
            foreach (var elemento in listaElementos)
            {
                if (elemento is Archivo a && a.CarpetaMadre == carpeta)
                    añadirElemento(a);
                else if (elemento is Carpeta c && c.CarpetaMadre == carpeta)
                    añadirElemento(c);
            }
        }

        private void Anterior_Click(object sender, RoutedEventArgs e)
        {
            if (carpetaActual.CarpetaMadre != null) abrir(carpetaActual.CarpetaMadre);
        }

        private string comprobarNombre(string nombre)
        {
            string baseName = System.IO.Path.GetFileNameWithoutExtension(nombre);
            string extension = System.IO.Path.GetExtension(nombre);

            int contador = 1;
            string nuevoNombre = nombre;

            bool existe;
            do
            {
                existe = false;

                foreach (Button item in WPExplorador.Children)
                {
                    if (item.Content?.ToString() == nuevoNombre)
                    {
                        nuevoNombre = $"{baseName} ({contador}){extension}";
                        contador++;
                        existe = true;
                        break;
                    }
                }

            } while (existe);

            return nuevoNombre;
        }

    }
}
