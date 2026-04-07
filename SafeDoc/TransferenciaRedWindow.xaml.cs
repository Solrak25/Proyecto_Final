using System;
using System.Windows;
using System.Windows.Input;
using System.Threading.Tasks;

namespace SafeDoc
{
    public partial class TransferenciaRedWindow : Window
    {
        private Carpeta carpetaAEnviar;
        public Carpeta? CarpetaRecibida { get; private set; }

        public TransferenciaRedWindow(Carpeta? carpeta = null)
        {
            InitializeComponent();
            carpetaAEnviar = carpeta ?? new Carpeta("Vacía");
            
            if (carpeta == null)
            {
                // Modo Receptor por defecto
                txtCodigoGenerado.Text = "Esperando código...";
            }
            else
            {
                // Modo Emisor
                string ip = GestorRed.ObtenerIPLocal();
                txtCodigoGenerado.Text = GestorRed.GenerarCodigo(ip);
            }
        }

        private void Log(string mensaje)
        {
            Dispatcher.Invoke(() => {
                txtLog.Text += $"\n> {mensaje}";
                scrollLog.ScrollToEnd();
            });
        }

        private void BtnCopiar_Click(object sender, RoutedEventArgs e)
        {
            if (txtCodigoGenerado.Text != "Esperando código...")
            {
                Clipboard.SetText(txtCodigoGenerado.Text);
                Log("¡Código copiado al portapapeles!");
            }
        }

        private void BtnCerrar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private async void BtnIniciarServidor_Click(object sender, RoutedEventArgs e)
        {
            btnIniciarServidor.IsEnabled = false;
            btnIniciarServidor.Content = "ESCUCHANDO...";
            try
            {
                Log("Iniciando socket TCP...");
                await Task.Run(() => GestorRed.EnviarCarpetaAsync(carpetaAEnviar, Log));
                Log("Transferencia terminada.");
                MessageBox.Show("¡Carpeta enviada con éxito!", "P2P", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                Log("Error: " + ex.Message);
            }
            finally
            {
                btnIniciarServidor.IsEnabled = true;
                btnIniciarServidor.Content = "INICIAR SERVIDOR DE ENVÍO";
            }
        }

        private async void BtnConectar_Click(object sender, RoutedEventArgs e)
        {
            string codigo = txtCodigoInput.Text.Trim();
            if (string.IsNullOrWhiteSpace(codigo)) return;

            btnConectar.IsEnabled = false;
            btnConectar.Content = "CONECTANDO...";
            try
            {
                string ip = GestorRed.ObtenerIPDesdeCodigo(codigo);
                Log($"Intentando conectar a {ip}...");
                
                var resultado = await Task.Run(() => GestorRed.RecibirCarpetaAsync(ip, Log));
                
                if (resultado != null)
                {
                    CarpetaRecibida = resultado;
                    Log("¡Carpeta recibida con éxito!");
                    MessageBox.Show("¡Carpeta recibida! Se añadirá a tu directorio actual.", "P2P", MessageBoxButton.OK, MessageBoxImage.Information);
                    DialogResult = true;
                    Close();
                }
                else
                {
                    Log("No se recibió nada o el emisor se desconectó.");
                }
            }
            catch (Exception ex)
            {
                Log("Error: " + ex.Message);
            }
            finally
            {
                btnConectar.IsEnabled = true;
                btnConectar.Content = "CONECTAR Y DESCARGAR";
            }
        }
    }
}
