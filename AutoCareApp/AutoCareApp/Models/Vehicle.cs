using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AutoCareApp.Models;

public partial class Vehicle : ObservableObject
{
    public int Id { get; set; }

    [ObservableProperty]
    public partial string Name { get; set; } = string.Empty;

    [ObservableProperty]
    public partial int CurrentMileage { get; set; } = 0;

    // Список всех записей обслуживания конкретно для этого автомобиля
    public ObservableCollection<ServiceRecord> Records { get; } = new();

    // Сумма расходов именно по этой машине
    public decimal TotalSpent => Records.Sum(r => r.TotalCost);

    public Vehicle()
    {
        // Слушаем добавление/удаление записей, чтобы сразу обновлять TotalSpent
        Records.CollectionChanged += (_, _) => OnPropertyChanged(nameof(TotalSpent));
    }
}
