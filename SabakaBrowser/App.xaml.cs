using System.Windows;
using CefSharp;
using CefSharp.Wpf;
using SabakaBrowser;

public partial class App : Application
{  
    protected override void OnStartup(StartupEventArgs e)
    {
        var settings = new CefSettings();
        settings.CefCommandLineArgs.Add("disable-gpu", "0");
        settings.CefCommandLineArgs.Add("disable-gpu-compositing", "0");
        settings.CefCommandLineArgs.Add("enable-gpu-rasterization", "1");
        settings.CefCommandLineArgs.Add("enable-zero-copy", "1");
        settings.CachePath = "CefCache";
        
        Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);

        base.OnStartup(e);
        var mainWindow = new MainWindow();
        mainWindow.Show();
    }
}