using System.Configuration;
using System.Data;
using System.Windows;

namespace CustomLightBrowser
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Mutex? _mutex = null;

        protected override void OnStartup(StartupEventArgs e)
        {
            // Ensure single instance
            const string appName = "LightBrowser_SingleInstance";
            bool createdNew;

            _mutex = new Mutex(true, appName, out createdNew);

            if (!createdNew)
            {
                // App is already running
                MessageBox.Show("LightBrowser is already running.", "LightBrowser",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                Environment.Exit(0);
                return;
            }

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _mutex?.ReleaseMutex();
            _mutex?.Dispose();
            base.OnExit(e);
        }

        private void Application_DispatcherUnhandledException(object sender,
            System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"An unexpected error occurred: {e.Exception.Message}",
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
    }

}
