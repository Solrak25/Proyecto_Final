using System.IO;
using System.Windows;
using System.Windows.Media;

namespace SafeDoc
{
    public partial class LoginWindow : Window
    {
        public string Username { get; private set; } = string.Empty;
        public string Password { get; private set; } = string.Empty;

        public LoginWindow()
        {
            InitializeComponent();
            
            // Verificamos si la base existe para dar la bienvenida o invitar a crear la primera cuenta
            if (!File.Exists("datos.dat"))
            {
                txtError.Text = "No hay usuarios registrados. Crea el primero.";
                txtError.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#60CDFF"));
            }
        }

        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
                this.DragMove();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string user = txtUsuario.Text.Trim();
                string pass = txtPassword.Password;

                if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass))
                {
                    txtError.Text = "Llena todos los campos.";
                    txtError.Foreground = new SolidColorBrush(Colors.Pink);
                    return;
                }

                if (GestorArchivos.ValidarLogin(user, pass))
                {
                    Username = user;
                    Password = pass;
                    DialogResult = true;
                    Close();
                }
                else
                {
                    txtError.Text = "Usuario o contraseña incorrectos.";
                    txtError.Foreground = new SolidColorBrush(Colors.Pink);
                }
            }
            catch (Exception ex)
            {
                txtError.Text = "Error: " + ex.Message;
                txtError.Foreground = new SolidColorBrush(Colors.Pink);
            }
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string user = txtUsuario.Text.Trim();
                string pass = txtPassword.Password;

                if (string.IsNullOrWhiteSpace(user) || user.Length < 3)
                {
                    txtError.Text = "Usuario muy corto.";
                    txtError.Foreground = new SolidColorBrush(Colors.Pink);
                    return;
                }

                if (string.IsNullOrWhiteSpace(pass) || pass.Length < 4)
                {
                    txtError.Text = "Contraseña débil (min 4 chars).";
                    txtError.Foreground = new SolidColorBrush(Colors.Pink);
                    return;
                }

                if (GestorArchivos.CrearUsuario(user, pass))
                {
                    Username = user;
                    Password = pass;
                    DialogResult = true;
                    Close();
                }
                else
                {
                    txtError.Text = "El usuario ya existe.";
                    txtError.Foreground = new SolidColorBrush(Colors.Pink);
                }
            }
            catch (Exception ex)
            {
                txtError.Text = "Excepción: " + ex.Message;
                txtError.Foreground = new SolidColorBrush(Colors.Pink);
            }
        }
    }
}