using CefSharp;
using CefSharp.Wpf;
using System.Windows;
using System.Windows.Input;

namespace SabakaBrowser
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            System.Windows.Media.RenderOptions.ProcessRenderMode = System.Windows.Interop.RenderMode.Default;

            Browser.Address = "https://www.google.com";
            AddressBar.Text = "https://www.google.com";
            
            Browser.BrowserSettings = new CefSharp.BrowserSettings
            {
                WindowlessFrameRate = 60,
                BackgroundColor = Cef.ColorSetARGB(255, 255, 255, 255)
            };

            WindowState = WindowState.Maximized;
        }

        private void AddressBar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Navigate(AddressBar.Text);
            }
        }

        private void Navigate(string url)
        {
            if (!url.StartsWith("http"))
                url = "https://" + url;

            Browser.Load(url);
        }
        
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Browser.CanGoBack)
                Browser.Back();
        }

        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            if (Browser.CanGoForward)
                Browser.Forward();
        }

        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            Browser.Reload();
        }
    }
}