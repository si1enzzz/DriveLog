using System;
using System.IO;
using Android.App;
using Android.Runtime;
using Avalonia;
using Avalonia.Android;

namespace AutoCareApp.Android;

[Application]
public class Application : AvaloniaAndroidApplication<AutoCareApp.App>
{
    public Application(nint javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
    {
    }

    public override void OnCreate()
    {
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        AndroidEnvironment.UnhandledExceptionRaiser += OnAndroidException;

        base.OnCreate();
    }

    private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        LogCrash("AppDomain", e.ExceptionObject as Exception);
    }

    private static void OnAndroidException(object? sender, RaiseThrowableEventArgs e)
    {
        LogCrash("AndroidEnvironment", e.Exception);
    }

    private static void LogCrash(string tag, Exception? ex)
    {
        if (ex == null) return;
        try
        {
            string line = "[" + DateTime.UtcNow + "] [" + tag + "] " + ex + "\n";
            global::Android.Util.Log.Error("AutoCare", line);

            var dir = global::Android.OS.Environment.GetExternalStoragePublicDirectory(global::Android.OS.Environment.DirectoryDownloads)?.AbsolutePath;
            if (!string.IsNullOrEmpty(dir))
            {
                File.AppendAllText(Path.Combine(dir, "AutoCare_Crash.txt"), line);
            }
        }
        catch { }
    }

    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        return base.CustomizeAppBuilder(builder)
            .WithInterFont();
    }
}