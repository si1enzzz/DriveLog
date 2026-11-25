using Android.App;
using Android.Content.PM;
using Android.Views;
using Avalonia.Android;

namespace DriveLog.Android;

[Activity(
    Label = "DriveLog",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@mipmap/appicon",
    MainLauncher = true,
    WindowSoftInputMode = SoftInput.AdjustResize,
    ConfigurationChanges =
        ConfigChanges.Orientation |
        ConfigChanges.ScreenSize |
        ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity
{
}