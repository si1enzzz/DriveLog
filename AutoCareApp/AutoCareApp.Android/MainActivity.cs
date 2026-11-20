using Android.App;
using Android.Content.PM;
using Android.Views;
using Avalonia.Android;

namespace AutoCareApp.Android;

[Activity(
    Label = "AutoCareApp",
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