using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CustomLightBrowser
{
    /// <summary>
    /// Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window
    {
        private readonly DispatcherTimer _timer;
        private readonly string[] _loadingMessages = {
            "Initializing...",
            "Loading WebView2...",
            "Setting up browser engine...",
            "Preparing user interface...",
            "Almost ready..."
        };
        private int _currentMessageIndex = 0;

        public SplashScreen()
        {
            InitializeComponent();

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();

            // Start the initialization process
            InitializeApplication();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_currentMessageIndex < _loadingMessages.Length)
            {
                StatusTextBlock.Text = _loadingMessages[_currentMessageIndex];
                _currentMessageIndex++;
            }
        }

        private async void InitializeApplication()
        {
            try
            {
                // Simulate initialization time (remove in production if not needed)
                await Task.Delay(2500);

                // Create and show main window
                var mainWindow = new MainWindow();

                // Close splash screen and show main window
                Application.Current.MainWindow = mainWindow;
                mainWindow.Show();

                _timer.Stop();
                this.Close();
            }
            catch (Exception ex)
            {
                _timer.Stop();
                MessageBox.Show($"Failed to initialize application: {ex.Message}",
                    "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            // Add fade-in animation
            this.Opacity = 0;
            var fadeIn = new System.Windows.Media.Animation.DoubleAnimation(0, 1,
                TimeSpan.FromMilliseconds(500));
            this.BeginAnimation(OpacityProperty, fadeIn);
        }
    }
}
