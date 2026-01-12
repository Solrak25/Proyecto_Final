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
        public List<object> listaElementos = new List<object>();

        private object elementoSeleccionado;
        private object clipboardElemento;

        public MainWindow()
        {
            InitializeComponent();

            Archivo archivo1 = new Archivo("Archivo1");
            Archivo archivo2 = new Archivo("Archivo2");
            Archivo archivo3 = new Archivo("Archivo3");
            Archivo archivo4 = new Archivo("Archivo4");
            Archivo archivo5 = new Archivo("Archivo5");
            Carpeta carpeta = new Carpeta("Carpeta1");

            añadirElemento(carpeta);
            añadirElemento(archivo1);
            añadirElemento(archivo2);
            añadirElemento(archivo3);
            añadirElemento(archivo4);
            añadirElemento(archivo5);
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
                añadirElemento(new Archivo(a.Nombre));

            // Si era Carpeta
            else if (clipboardElemento is Carpeta c)
                añadirElemento(new Carpeta(c.Nombre));
        }

        public void añadirElemento(object elemento)
        {
            listaElementos.Add(elemento);

            Button btn = new Button();
            btn.Background = Brushes.Transparent;
            btn.Tag = elemento;
            btn.Click += btnElemento_Click;

            // texto mostrado
            if (elemento is Archivo a)
            {
                btn.Content = a.Nombre;
                btn.Template = (ControlTemplate)FindResource("Archivos");
            }
            else if (elemento is Carpeta c)
            {
                btn.Content = c.Nombre;
                btn.Template = (ControlTemplate)FindResource("Carpetas");
            }

            WPExplorador.Children.Add(btn);
        }

        public void eliminarElemento(object elemento)
        {
            int id = listaElementos.IndexOf(elemento);
            listaElementos.Remove(elemento);
            WPExplorador.Children.RemoveAt(id);
        }

        private void btnElemento_Click(object sender, RoutedEventArgs e)
        {
            foreach (Button item in WPExplorador.Children)
                item.Background = Brushes.Transparent;

            elementoSeleccionado = ((Button)sender).Tag;

            Button btnSeleccionado = sender as Button;
            btnSeleccionado.Background = Brushes.LightBlue;
        }
    }

}