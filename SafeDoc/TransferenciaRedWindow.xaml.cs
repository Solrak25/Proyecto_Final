using System;
using System.Windows;
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
                // txtCodigoGenerado.Visibility = Visibility.Collapsed;
            }
            else
            {
                string ip = GestorRed.ObtenerIPLocal();
                txtCodigoGenerado.Text = GestorRed.GenerarCodigo(ip);
            }
        }

        private void Log(string mensaje)
        {
            Dispatcher.Invoke(() => {
                txtLog.Text += $"\n[{DateTime.Now:HH:mm:ss}] {mensaje}";
            });
        }

        private async void BtnIniciarServidor_Click(object sender, RoutedEventArgs e)
        {
            btnIniciarServidor.IsEnabled = false;
            try
            {
                Log("Iniciando servidor P2P...");
                await Task.Run(() => GestorRed.EnviarCarpetaAsync(carpetaAEnviar, Log));
                Log("Transferencia terminada.");
            }
            catch (Exception ex)
            {
                Log("Error: " + ex.Message);
            }
            finally
            {
                btnIniciarServidor.IsEnabled = true;
            }
        }

        private async void BtnConectar_Click(object sender, RoutedEventArgs e)
        {
            string codigo = txtCodigoInput.Text.Trim();
            if (string.IsNullOrWhiteSpace(codigo)) return;

            btnConectar.IsEnabled = false;
            try
            {
                string ip = GestorRed.ObtenerIPDesdeCodigo(codigo);
                Log($"Intentando conectar a {ip}...");
                
                var resultado = await Task.Run(() => GestorRed.RecibirCarpetaAsync(ip, Log));
                
                if (resultado != null)
                {
                    CarpetaRecibida = resultado;
                    Log("¡Carpeta recibida con éxito!");
                    MessageBox.Show("¡Carpeta recibida! Se añadirá a tu directorio actual.");
                    DialogResult = true;
                    Close();
                }
                else
                {
                    Log("No se recibió nada.");
                }
            }
            catch (Exception ex)
            {
                Log("Error de conexión: " + ex.Message);
            }
            finally
            {
                btnConectar.IsEnabled = true;
            }
        }
    }
}
