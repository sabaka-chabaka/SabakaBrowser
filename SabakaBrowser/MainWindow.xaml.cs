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

            Browser.Address = "https://www.google.com";
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
    }
}