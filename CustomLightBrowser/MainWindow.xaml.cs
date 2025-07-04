using System.ComponentModel;
using System.Net.NetworkInformation;
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
using Microsoft.Web.WebView2.Core;

namespace CustomLightBrowser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string _homePage = "https://www.google.com";
        private bool _isNavigating = false;

        public MainWindow()
        {
            InitializeComponent();
            InitializeAsync();

            // Monitor network connectivity
            NetworkChange.NetworkAvailabilityChanged += OnNetworkAvailabilityChanged;

            // Set initial status
            UpdateConnectionStatus();
        }

        private async void InitializeAsync()
        {
            try
            {
                await webView.EnsureCoreWebView2Async();

                // Configure WebView2 settings
                webView.CoreWebView2.Settings.IsStatusBarEnabled = false;
                webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;
                webView.CoreWebView2.Settings.AreHostObjectsAllowed = false;
                webView.CoreWebView2.Settings.IsWebMessageEnabled = false;
                webView.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = true;

                // Subscribe to additional events
                webView.CoreWebView2.DocumentTitleChanged += CoreWebView2_DocumentTitleChanged;
                webView.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;

                // Navigate to home page
                webView.CoreWebView2.Navigate(_homePage);
                AddressBar.Text = _homePage;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to initialize browser: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (webView.CoreWebView2?.CanGoBack == true)
            {
                webView.CoreWebView2.GoBack();
            }
        }

        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            if (webView.CoreWebView2?.CanGoForward == true)
            {
                webView.CoreWebView2.GoForward();
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            webView.CoreWebView2?.Reload();
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            webView.CoreWebView2?.Navigate(_homePage);
        }

        private void AddressBar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                NavigateToUrl(AddressBar.Text);
            }
        }

        private void AddressBar_GotFocus(object sender, RoutedEventArgs e)
        {
            AddressBar.SelectAll();
        }

        private void NavigateToUrl(string url)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(url))
                    return;

                // Add protocol if missing
                if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                {
                    // Check if it looks like a URL
                    if (url.Contains(".") && !url.Contains(" "))
                    {
                        url = "https://" + url;
                    }
                    else
                    {
                        // Treat as search query
                        url = $"https://www.google.com/search?q={Uri.EscapeDataString(url)}";
                    }
                }

                webView.CoreWebView2?.Navigate(url);
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Navigation error: {ex.Message}";
            }
        }

        private void WebView_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            _isNavigating = true;
            StatusText.Text = "Loading...";
            RefreshButton.Content = "✕";

            // Update security indicator
            UpdateSecurityIndicator(e.Uri);
        }

        private void WebView_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            _isNavigating = false;
            RefreshButton.Content = "⟳";

            if (e.IsSuccess)
            {
                StatusText.Text = "Ready";
                AddressBar.Text = webView.CoreWebView2.Source;

                // Update navigation buttons
                BackButton.IsEnabled = webView.CoreWebView2.CanGoBack;
                ForwardButton.IsEnabled = webView.CoreWebView2.CanGoForward;
            }
            else
            {
                StatusText.Text = "Failed to load page";
                ShowOfflineContent();
            }
        }

        private void WebView_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                // WebView2 is fully initialized
                StatusText.Text = "Browser initialized";
            }
            else
            {
                StatusText.Text = "Failed to initialize browser";
                MessageBox.Show($"WebView2 initialization failed: {e.InitializationException?.Message}",
                    "Initialization Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CoreWebView2_DocumentTitleChanged(object sender, object e)
        {
            // Update window title when page title changes
            if (!string.IsNullOrEmpty(webView.CoreWebView2.DocumentTitle))
            {
                Title = $"{webView.CoreWebView2.DocumentTitle} - LightBrowser";
            }
        }

        private void CoreWebView2_NewWindowRequested(object sender, CoreWebView2NewWindowRequestedEventArgs e)
        {
            // Handle new window requests - open in same window
            e.Handled = true;
            webView.CoreWebView2.Navigate(e.Uri);
        }

        private void UpdateSecurityIndicator(string url)
        {
            if (url.StartsWith("https://"))
            {
                SecurityIndicator.Text = "🔒";
                SecurityIndicator.Foreground = System.Windows.Media.Brushes.Green;
            }
            else if (url.StartsWith("http://"))
            {
                SecurityIndicator.Text = "⚠️";
                SecurityIndicator.Foreground = System.Windows.Media.Brushes.Orange;
            }
            else
            {
                SecurityIndicator.Text = "ℹ️";
                SecurityIndicator.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            var contextMenu = new System.Windows.Controls.ContextMenu();

            var newWindowItem = new System.Windows.Controls.MenuItem { Header = "New Window" };
            newWindowItem.Click += (s, args) => new MainWindow().Show();

            var settingsItem = new System.Windows.Controls.MenuItem { Header = "Settings" };
            settingsItem.Click += (s, args) => ShowSettings();

            var aboutItem = new System.Windows.Controls.MenuItem { Header = "About" };
            aboutItem.Click += (s, args) => ShowAbout();

            contextMenu.Items.Add(newWindowItem);
            contextMenu.Items.Add(new System.Windows.Controls.Separator());
            contextMenu.Items.Add(settingsItem);
            contextMenu.Items.Add(aboutItem);

            contextMenu.PlacementTarget = MenuButton;
            contextMenu.IsOpen = true;
        }

        private void ShowSettings()
        {
            MessageBox.Show("Settings functionality coming soon!", "Settings",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowAbout()
        {
            MessageBox.Show("LightBrowser v1.0\n\nA lightweight, fast web browser built with WPF and WebView2.\n\n© 2025 Harmander Singh",
                "About LightBrowser", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OnNetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                UpdateConnectionStatus();

                if (!e.IsAvailable)
                {
                    ShowOfflineContent();
                }
                else
                {
                    HideOfflineContent();
                }
            });
        }

        private void UpdateConnectionStatus()
        {
            bool isOnline = NetworkInterface.GetIsNetworkAvailable();
            ConnectionStatus.Text = isOnline ? "Online" : "Offline";
            ConnectionStatus.Foreground = isOnline ?
                System.Windows.Media.Brushes.Green :
                System.Windows.Media.Brushes.Red;
        }

        private void ShowOfflineContent()
        {
            OfflineOverlay.Visibility = Visibility.Visible;
        }

        private void HideOfflineContent()
        {
            OfflineOverlay.Visibility = Visibility.Collapsed;
        }

        private void TryAgainButton_Click(object sender, RoutedEventArgs e)
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                HideOfflineContent();
                webView.CoreWebView2?.Reload();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            // Unsubscribe from events
            NetworkChange.NetworkAvailabilityChanged -= OnNetworkAvailabilityChanged;

            if (webView.CoreWebView2 != null)
            {
                webView.CoreWebView2.DocumentTitleChanged -= CoreWebView2_DocumentTitleChanged;
                webView.CoreWebView2.NewWindowRequested -= CoreWebView2_NewWindowRequested;
            }

            base.OnClosing(e);
        }
    }
}