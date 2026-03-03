using System;
using CefSharp;
using CefSharp.Wpf;
using System.Windows;
using System.Windows.Input;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace SabakaBrowser
{
    public partial class MainWindow : Window
    {
        private List<HistoryItem> _history = new();
        private string historyFile = "history.json";
        
        private ChromiumWebBrowser? GetCurrentBrowser()
        {
            if (BrowserTabs.SelectedItem is TabItem tab &&
                tab.Content is ChromiumWebBrowser browser)
                return browser;

            return null;
        }
        
        public MainWindow()
        {
            InitializeComponent();

            System.Windows.Media.RenderOptions.ProcessRenderMode = System.Windows.Interop.RenderMode.Default;

            AddressBar.Text = "https://www.google.com";

            WindowState = WindowState.Maximized;
            
            LoadHistory();
            
            AddNewTab();
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
        
        private void AddTab_Click(object sender, RoutedEventArgs e)
        {
            AddNewTab();
        }
        
        private void AddToHistory(string url, string title)
        {
            if (string.IsNullOrEmpty(url))
                return;

            // защита от дублей подряд
            if (_history.Count > 0 && _history[1].Url == url)
                return;

            _history.Add(new HistoryItem
            {
                Url = url,
                Title = title,
                VisitTime = DateTime.Now
            });

            SaveHistory();
        }
        
        private void AddNewTab(string url = "https://google.com")
        {
            var browser = new ChromiumWebBrowser(url);

            var headerPanel = new StackPanel { Orientation = Orientation.Horizontal,  };
            var titleBlock = new TextBlock
            {
                Text = "Новая вкладка",
                Margin = new Thickness(0, 0, 5, 0)
            };

            var closeButton = new Button
            {
                Content = "✕",
                Width = 20,
                Height = 20,
                Padding = new Thickness(0),
                Margin = new Thickness(0),
                Background = null,
                BorderThickness = new Thickness(0),
                Cursor = Cursors.Hand
            };

            headerPanel.Children.Add(titleBlock);
            headerPanel.Children.Add(closeButton);
            
            var tab = new TabItem
            {
                Header = "Новая вкладка",
                Content = browser
            };

            BrowserTabs.Items.Add(tab);
            BrowserTabs.SelectedItem = tab;

            // Title
            browser.TitleChanged += (s, e) =>
            {
                Dispatcher.Invoke(() =>
                {
                    var title = e.NewValue?.ToString() ?? "Новая вкладка";
                    tab.Header = title;

                    if (BrowserTabs.SelectedItem == tab)
                        this.Title = $"{title} - Sabaka Browser";

                    AddToHistory(browser.Address, title);
                });
            };

            // Address
            browser.AddressChanged += (s, e) =>
            {
                Dispatcher.Invoke(() =>
                {
                    if (BrowserTabs.SelectedItem == tab)
                        AddressBar.Text = e.NewValue.ToString();
                });
            };
            
            closeButton.Click += (s, e) =>
            {
                // Освобождаем ресурсы браузера
                browser.Dispose();
                BrowserTabs.Items.Remove(tab);
            };

        }

        private void AddressBar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var browser = GetCurrentBrowser();
                if (browser == null) return;

                var url = AddressBar.Text;

                if (Uri.TryCreate(url, UriKind.Absolute, out Uri result) &&
                    (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps))
                {
                    if (!url.StartsWith("http"))
                        url = "https://" + url;

                    browser.Load(url);
                }
                else if (IsLikelyDomain(url))
                {
                    if (!url.StartsWith("http"))
                        url = "https://" + url;

                    browser.Load(url);
                }
                else
                {
                    browser.Load("https://www.google.com/search?q=" + Uri.EscapeDataString(url));
                }
            }
        }
        
        private bool IsLikelyDomain(string input)
        {
            return input.Contains(".") && !input.Contains(" ");
        }
        
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var browser = GetCurrentBrowser();
            if (browser?.CanGoBack == true)
                browser.Back();
        }

        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            var browser = GetCurrentBrowser();
            if (browser?.CanGoForward == true)
                browser.Forward();
        }

        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            GetCurrentBrowser()?.Reload();
        }
        
        private void History_Click(object sender, RoutedEventArgs e)
        {
            var window = new HistoryWindow(_history);

            if (window.ShowDialog() == true && window.SelectedUrl != null)
            {
                GetCurrentBrowser()?.Load(window.SelectedUrl);
            }
        }
    }
}