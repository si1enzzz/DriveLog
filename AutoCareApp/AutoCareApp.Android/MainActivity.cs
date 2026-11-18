using Android.App;
using Android.Content.PM;
using Avalonia.Android;

namespace AutoCareApp.Android;

[Activity(
    Label = "AutoCareApp",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@mipmap/appicon",
    MainLauncher = true,
    ConfigurationChanges =
        ConfigChanges.Orientation |
        ConfigChanges.ScreenSize |
        ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity
{
}