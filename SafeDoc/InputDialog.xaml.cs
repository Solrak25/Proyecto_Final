using System.Windows;
using System.Windows.Input;

namespace SafeDoc
{
    public partial class InputDialog : Window
    {
        public string Respuesta { get; private set; } = string.Empty;

        public InputDialog(string titulo)
        {
            InitializeComponent();
            txtTitulo.Text = titulo;
            txtEntrada.Focus();
        }

        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            Respuesta = txtEntrada.Text;
            DialogResult = true;
            Close();
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove(); // Allow dragging
        }
    }
}
