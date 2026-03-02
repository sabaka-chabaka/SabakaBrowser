using System.Windows;
using System.Collections.Generic;

namespace SabakaBrowser;

public partial class HistoryWindow : Window
{
    private List<HistoryItem> _history;

    public string? SelectedUrl { get; private set; }

    public HistoryWindow(List<HistoryItem> history)
    {
        InitializeComponent();
        _history = history;

        HistoryList.ItemsSource = _history;
        HistoryList.DisplayMemberPath = "Title";
    }

    private void HistoryList_DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (HistoryList.SelectedItem is HistoryItem item)
        {
            SelectedUrl = item.Url;
            DialogResult = true;
            Close();
        }
    }
}