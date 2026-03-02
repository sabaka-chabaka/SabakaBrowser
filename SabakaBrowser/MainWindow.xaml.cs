using System;
using CefSharp;
using CefSharp.Wpf;
using System.Windows;
using System.Windows.Input;
using System.Text;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SabakaBrowser
{
    public partial class MainWindow : Window
    {
        private List<HistoryItem> _history = new();
        private string historyFile = "history.json";
        
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

            Browser.AddressChanged += Browser_AddressChanged;

            WindowState = WindowState.Maximized;
            
            LoadHistory();
            
            Browser.TitleChanged += Browser_TitleChanged;
            Browser.LoadingStateChanged += Browser_LoadingStateChanged;
        }

        private void LoadHistory()
        {
            if (File.Exists(historyFile))
            {
                var json = File.ReadAllText(historyFile);

                if (!string.IsNullOrWhiteSpace(json))
                {
                    _history = JsonConvert.DeserializeObject<List<HistoryItem>>(json) 
                               ?? new List<HistoryItem>();
                }
            }
        }
        
        private void SaveHistory()
        {
            var json = JsonConvert.SerializeObject(_history, Formatting.Indented);
            File.WriteAllText(historyFile, json);
        }
        
        
        
        private void Browser_LoadingStateChanged(object sender, CefSharp.LoadingStateChangedEventArgs e)
        {
            if (!e.IsLoading)
            {
                Dispatcher.Invoke(() =>
                {
                    AddressBar.Text = Browser.Address;
                });
            }
        }
        
        private void Browser_AddressChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            AddressBar.Text = e.NewValue.ToString();
        }
        
        private void Browser_TitleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var title = e.NewValue?.ToString();
            var url = Browser.Address;

            if (string.IsNullOrEmpty(url))
                return;

            Dispatcher.Invoke(() =>
            {
                this.Title = $"{title} - Sabaka Browser"; // Заголовок окна

                _history.Add(new HistoryItem
                {
                    Url = url,
                    Title = title,
                    VisitTime = DateTime.Now
                });

                SaveHistory();
            });
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
        
        private void History_Click(object sender, RoutedEventArgs e)
        {
            var window = new HistoryWindow(_history);

            if (window.ShowDialog() == true && window.SelectedUrl != null)
            {
                Browser.Load(window.SelectedUrl);
            }
        }
    }
}