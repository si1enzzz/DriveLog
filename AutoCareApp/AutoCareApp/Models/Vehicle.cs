using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AutoCareApp.Models;

public partial class Vehicle : ObservableObject
{
    public int Id { get; set; }

    // Марка (например, Lada, Kia, Toyota)
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplayName))]
    public partial string Brand { get; set; } = string.Empty;

    // Модель (например, Vesta, Rio, Camry)
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplayName))]
    public partial string Model { get; set; } = string.Empty;

    // Поколение (например, I, II, Рестайлинг, 2015-2020)
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SubDetails))]
    public partial string Generation { get; set; } = string.Empty;

    // Год выпуска
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplayName))]
    [NotifyPropertyChangedFor(nameof(SubDetails))]
    public partial int Year { get; set; } = DateTime.Today.Year;

    // Объем двигателя (например: 1.6, 2.0, 1.5T)
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplayName))]
    [NotifyPropertyChangedFor(nameof(SubDetails))]
    public partial string EngineVolume { get; set; } = string.Empty;

    // Цвет автомобиля (например, Черный, Белый, Серебристый)
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SubDetails))]
    public partial string Color { get; set; } = string.Empty;

    // Текущий пробег
    [ObservableProperty]
    public partial int CurrentMileage { get; set; } = 0;

    // Главное красивое название для шапки и списка
    public string DisplayName
    {
        get
        {
            var parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(Brand)) parts.Add(Brand);
            if (!string.IsNullOrWhiteSpace(Model)) parts.Add(Model);
            if (!string.IsNullOrWhiteSpace(EngineVolume)) parts.Add($"{EngineVolume}L");
            if (Year > 1900) parts.Add($"({Year})");

            return parts.Count > 0 ? string.Join(" ", parts) : "Автомобиль";
        }
    }

    // Дополнительные параметры (поколение, двигатель, цвет)
    public string SubDetails
    {
        get
        {
            var details = new List<string>();
            if (!string.IsNullOrWhiteSpace(Generation)) details.Add(Generation);
            if (!string.IsNullOrWhiteSpace(EngineVolume)) details.Add($"Двигатель: {EngineVolume} л");
            if (Year > 1900) details.Add($"{Year} г.в.");
            if (!string.IsNullOrWhiteSpace(Color)) details.Add($"Цвет: {Color}");

            return string.Join(" · ", details);
        }
    }

    // История замен конкретно этого автомобиля
    public ObservableCollection<ServiceRecord> Records { get; } = new();

    // Общие расходы по автомобилю
    public decimal TotalSpent => Records.Sum(r => r.TotalCost);

    public Vehicle()
    {
        Records.CollectionChanged += (_, _) => OnPropertyChanged(nameof(TotalSpent));
    }
}
