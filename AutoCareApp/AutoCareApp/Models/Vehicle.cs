using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AutoCareApp.Models;

public partial class Vehicle : ObservableObject
{
    public int Id { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Title))]
    public partial string Brand { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Title))]
    public partial string Model { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SubDetails))]
    public partial string Generation { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SubDetails))]
    public partial int Year { get; set; } = DateTime.Today.Year;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SubDetails))]
    public partial string EngineVolume { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SubDetails))]
    public partial string Color { get; set; } = string.Empty;

    [ObservableProperty]
    public partial int CurrentMileage { get; set; } = 0;

    // Только марка и модель для крупного заголовка (например: "Hyundai Elantra")
    public string Title
    {
        get
        {
            var parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(Brand)) parts.Add(Brand);
            if (!string.IsNullOrWhiteSpace(Model)) parts.Add(Model);

            return parts.Count > 0 ? string.Join(" ", parts) : "Автомобиль";
        }
    }

    // Вся детальная информация в одной аккуратной строке под заголовком
    public string SubDetails
    {
        get
        {
            var details = new List<string>();
            if (!string.IsNullOrWhiteSpace(Generation)) details.Add(Generation);
            if (!string.IsNullOrWhiteSpace(EngineVolume)) details.Add($"{EngineVolume} л");
            if (Year > 1900) details.Add($"{Year} г.в.");
            if (!string.IsNullOrWhiteSpace(Color)) details.Add(Color);

            return details.Count > 0 ? string.Join("  ·  ", details) : "Характеристики не указаны";
        }
    }

    // Список записей обслуживания
    public ObservableCollection<ServiceRecord> Records { get; } = new();

    // Сумма расходов по этой машине
    public decimal TotalSpent => Records.Sum(r => r.TotalCost);

    public Vehicle()
    {
        Records.CollectionChanged += (_, _) => OnPropertyChanged(nameof(TotalSpent));
    }
}
