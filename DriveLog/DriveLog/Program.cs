using Avalonia;
using System;

namespace DriveLog;

public sealed class Program
{
#if !ANDROID
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);
#endif

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
#if ANDROID
            .UseAndroid()
#else
            .UsePlatformDetect()
#if DEBUG
            .WithDeveloperTools()
#endif
#endif
            .WithInterFont()
            .LogToTrace();
}
