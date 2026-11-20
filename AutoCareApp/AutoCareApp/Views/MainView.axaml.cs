using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Styling;
using Avalonia.VisualTree;
using AutoCareApp.ViewModels;

namespace AutoCareApp.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();

        // Открывать выпадающий список сразу при фокусе или клике на любое поле подсказок
        AddHandler(GotFocusEvent, OnControlGotFocus);
        AddHandler(PointerReleasedEvent, OnControlPointerReleased, RoutingStrategies.Tunnel);
    }

    private void OnControlGotFocus(object? sender, RoutedEventArgs e)
    {
        TryOpenAutoCompleteDropDown(e.Source as Visual);
    }

    private void OnControlPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        TryOpenAutoCompleteDropDown(e.Source as Visual);
    }

    private static void TryOpenAutoCompleteDropDown(Visual? visual)
    {
        if (visual == null) return;
        var autoBox = visual.FindAncestorOfType<AutoCompleteBox>(includeSelf: true);
        if (autoBox != null)
        {
            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                autoBox.IsDropDownOpen = true;
            }, Avalonia.Threading.DispatcherPriority.Input);
        }
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        ApplySystemTheme();

        if (Application.Current != null)
        {
            Application.Current.ActualThemeVariantChanged += OnSystemThemeChanged;
        }
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        if (Application.Current != null)
        {
            Application.Current.ActualThemeVariantChanged -= OnSystemThemeChanged;
        }
    }

    private void OnSystemThemeChanged(object? sender, EventArgs e)
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(ApplySystemTheme);
    }

    private void ApplySystemTheme()
    {
        try
        {
            if (DataContext is MainViewModel vm && Application.Current != null)
            {
                var theme = Application.Current.ActualThemeVariant;
                if (theme == ThemeVariant.Light)
                {
                    vm.IsDarkTheme = false;
                }
                else if (theme == ThemeVariant.Dark)
                {
                    vm.IsDarkTheme = true;
                }
            }
        }
        catch { }
    }
}