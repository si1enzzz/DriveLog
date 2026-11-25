using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using DriveLog.ViewModels;

namespace DriveLog.Views;

public partial class MainView : UserControl
{
    private readonly Dictionary<ScrollViewer, DispatcherTimer> _scrollHideTimers = new();
    private readonly HashSet<ScrollViewer> _hookedScrollViewers = new();

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
            Dispatcher.UIThread.Post(() =>
            {
                autoBox.IsDropDownOpen = true;
            }, DispatcherPriority.Input);
        }
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        ApplySystemTheme();
        SetupScrollAutoHide();

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

        foreach (var timer in _scrollHideTimers.Values)
        {
            timer.Stop();
        }
        _scrollHideTimers.Clear();

        foreach (var sv in _hookedScrollViewers)
        {
            sv.ScrollChanged -= OnScrollViewerScrollChanged;
            sv.PropertyChanged -= OnScrollViewerPropertyChanged;
        }
        _hookedScrollViewers.Clear();
    }

    private void SetupScrollAutoHide()
    {
        foreach (var sv in this.GetVisualDescendants().OfType<ScrollViewer>())
        {
            if (!_hookedScrollViewers.Add(sv)) continue;

            sv.ScrollChanged += OnScrollViewerScrollChanged;
            sv.PropertyChanged += OnScrollViewerPropertyChanged;

            if (sv.IsVisible)
            {
                Dispatcher.UIThread.Post(() => ShowScrollBarTemporarily(sv), DispatcherPriority.Loaded);
            }
        }
    }

    private void OnScrollViewerPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == IsVisibleProperty && sender is ScrollViewer sv && sv.IsVisible)
        {
            Dispatcher.UIThread.Post(() => ShowScrollBarTemporarily(sv), DispatcherPriority.Loaded);
        }
    }

    private void OnScrollViewerScrollChanged(object? sender, ScrollChangedEventArgs e)
    {
        if (sender is ScrollViewer sv && e.OffsetDelta != Vector.Zero)
        {
            ShowScrollBarTemporarily(sv);
        }
    }

    private void ShowScrollBarTemporarily(ScrollViewer sv)
    {
        foreach (var sb in sv.GetVisualDescendants().OfType<ScrollBar>())
        {
            if (sb.FindAncestorOfType<ScrollViewer>() == sv)
            {
                sb.Opacity = 1.0;
            }
        }

        if (!_scrollHideTimers.TryGetValue(sv, out var timer))
        {
            timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
            timer.Tick += (_, _) =>
            {
                timer.Stop();
                foreach (var sb in sv.GetVisualDescendants().OfType<ScrollBar>())
                {
                    if (sb.FindAncestorOfType<ScrollViewer>() == sv)
                    {
                        sb.Opacity = 0.0;
                    }
                }
            };
            _scrollHideTimers[sv] = timer;
        }

        timer.Stop();
        timer.Start();
    }

    private void OnSystemThemeChanged(object? sender, EventArgs e)
    {
        Dispatcher.UIThread.Post(ApplySystemTheme);
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
