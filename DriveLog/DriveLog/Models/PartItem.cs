using CommunityToolkit.Mvvm.ComponentModel;

namespace DriveLog.Models;

public partial class PartItem : ObservableObject
{
    [ObservableProperty]
    public partial string Title { get; set; } = string.Empty;

    [ObservableProperty]
    public partial decimal? Cost { get; set; } = null;
}