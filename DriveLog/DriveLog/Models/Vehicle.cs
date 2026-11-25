using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

namespace DriveLog.Models;

public partial class Vehicle : ObservableObject
{
    [PrimaryKey, AutoIncrement]
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
    public partial int Horsepower { get; set; } = 0;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SubDetails))]
    public partial string Color { get; set; } = string.Empty;

    [ObservableProperty]
    public partial int CurrentMileage { get; set; } = 0;

    // Только марка и модель для крупного заголовка (например: "Hyundai Elantra")
    [Ignore]
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
    [Ignore]
    public string SubDetails
    {
        get
        {
            var details = new List<string>();
            if (!string.IsNullOrWhiteSpace(Generation)) details.Add(Generation);
            if (!string.IsNullOrWhiteSpace(EngineVolume)) details.Add($"{EngineVolume} л");
            if (Horsepower > 0) details.Add($"{Horsepower} л.с.");
            if (Year > 1900) details.Add($"{Year} г.в.");
            if (!string.IsNullOrWhiteSpace(Color)) details.Add(Color);

            return details.Count > 0 ? string.Join("  ·  ", details) : "Характеристики не указаны";
        }
    }

    // Список записей обслуживания
    [Ignore]
    public ObservableCollection<ServiceRecord> Records { get; } = new();

    // Сумма расходов по этой машине
    [Ignore]
    public decimal TotalSpent => Records.Sum(r => r.TotalCost);

    public Vehicle()
    {
        Records.CollectionChanged += (_, _) => OnPropertyChanged(nameof(TotalSpent));
    }

    public void NotifyTotalSpentChanged() => OnPropertyChanged(nameof(TotalSpent));
}
