using System;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AutoCareApp.ViewModels;
using AutoCareApp.Views;

namespace AutoCareApp;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        try
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainViewModel(),
                };
            }
            else if (ApplicationLifetime is IActivityApplicationLifetime activityLifetime)
            {
                activityLifetime.MainViewFactory = () =>
                {
                    try
                    {
                        return new MainView
                        {
                            DataContext = new MainViewModel(),
                        };
                    }
                    catch (Exception ex)
                    {
                        LogToFile("MainViewFactory_Error", ex);
                        return BuildErrorFallback(ex);
                    }
                };
            }
            else if (ApplicationLifetime is ISingleViewApplicationLifetime singleView)
            {
                try
                {
                    singleView.MainView = new MainView
                    {
                        DataContext = new MainViewModel(),
                    };
                }
                catch (Exception ex)
                {
                    LogToFile("SingleView_Error", ex);
                    singleView.MainView = BuildErrorFallback(ex);
                }
            }
        }
        catch (Exception ex)
        {
            LogToFile("OnFrameworkInitializationCompleted_Error", ex);
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static Control BuildErrorFallback(Exception ex)
    {
        return new ScrollViewer
        {
            Background = Avalonia.Media.Brushes.Black,
            Content = new TextBlock
            {
                Text = "ОШИБКА ЗАПУСКА AUTOCARE:\n\n" + ex.GetType().FullName + ": " + ex.Message + "\n\nСтек:\n" + ex.StackTrace + "\n\nInner: " + ex.InnerException?.Message,
                Foreground = Avalonia.Media.Brushes.OrangeRed,
                FontSize = 13,
                TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                Margin = new Thickness(15)
            }
        };
    }

    private static void LogToFile(string tag, Exception ex)
    {
        try
        {
            string line = "[" + DateTime.UtcNow + "] [" + tag + "] " + ex + "\n";
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "crash.txt");
            File.AppendAllText(path, line);
        }
        catch { }
    }
}
