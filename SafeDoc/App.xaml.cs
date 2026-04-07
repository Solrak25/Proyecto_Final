using System.Windows;

namespace SafeDoc
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            LoginWindow login = new LoginWindow();

            if (login.ShowDialog() == true)
            {
                MainWindow main = new MainWindow(login.Username, login.Password);
                Application.Current.MainWindow = main;
                main.Show();
                
                Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            }
            else
            {
                Shutdown();
            }
        }
    }
}