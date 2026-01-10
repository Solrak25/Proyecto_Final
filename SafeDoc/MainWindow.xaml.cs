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

namespace SafeDoc
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public List<Archivo> listaArchivos = new List<Archivo>();

        private Archivo archivoSeleccionado;


        public MainWindow()
        {
            InitializeComponent();

            Archivo archivo1 = new Archivo("Archivo1");
            Archivo archivo2 = new Archivo("Archivo2");
            Archivo archivo3 = new Archivo("Archivo3");
            Archivo archivo4 = new Archivo("Archivo4");
            Archivo archivo5 = new Archivo("Archivo5");
            añadirArchivo(archivo1);
            añadirArchivo(archivo2);
            añadirArchivo(archivo3);
            añadirArchivo(archivo4);
            añadirArchivo(archivo5);
        }

        private Archivo clipboardArchivo;
        private bool isCut = false;

        private void Copy_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (archivoSeleccionado == null) return;

            clipboardArchivo = archivoSeleccionado;
        }

        private void Cut_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (archivoSeleccionado == null) return;

            clipboardArchivo = archivoSeleccionado;
            eliminarArchivo(archivoSeleccionado);
        }

        private void Paste_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (clipboardArchivo == null) return;

            Archivo nuevo = new Archivo(clipboardArchivo.Nombre);
            añadirArchivo(nuevo);
        }


        public void añadirArchivo(Archivo archivo)
        {
            listaArchivos.Add(archivo);

            Button btnArchivo = new Button();
            btnArchivo.Width = 60;
            btnArchivo.Height = 75;
            btnArchivo.Margin = new Thickness(20);
            btnArchivo.Content = archivo.Nombre;

            btnArchivo.Tag = archivo;
            btnArchivo.Click += btnArchivo_Click;

            WPExplorador.Children.Add(btnArchivo);
        }


        public void eliminarArchivo(Archivo archivo)
        {
            int id = listaArchivos.IndexOf(archivo);
            listaArchivos.Remove(archivo);
            WPExplorador.Children.RemoveAt(id);
        }

        private void btnArchivo_Click(object sender, RoutedEventArgs e)
        {
            foreach(Button item in WPExplorador.Children)
            {
                item.Background = Brushes.LightGray;
            }
            archivoSeleccionado = (Archivo)((Button)sender).Tag;
            Button BtnSeleccionado = sender as Button;
            BtnSeleccionado.Background = Brushes.LightBlue;
        }
    }
}