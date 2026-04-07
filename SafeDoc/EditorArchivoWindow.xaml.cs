using System.Windows;
using System.Windows.Input;

namespace SafeDoc
{
    public partial class EditorArchivoWindow : Window
    {
        private Archivo archivoActual;

        public EditorArchivoWindow(Archivo archivo)
        {
            InitializeComponent();
            archivoActual = archivo;
            
            txtTitulo.Text = $"📄 Bloc Protegido - {archivo.Nombre}";
            txtContenido.Text = archivo.ContenidoTexto;
            
            txtContenido.Focus();
            txtContenido.CaretIndex = txtContenido.Text.Length;
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            archivoActual.ContenidoTexto = txtContenido.Text;
            DialogResult = true;
            Close();
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
