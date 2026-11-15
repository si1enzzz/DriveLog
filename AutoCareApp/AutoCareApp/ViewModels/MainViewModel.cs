using System;
using System.Collections.ObjectModel;
using System.Linq;
using AutoCareApp.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AutoCareApp.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    // Название автомобиля
    [ObservableProperty]
    public partial string CarName { get; set; } = "Lada Vesta 1.6";

    // Текущий пробег автомобиля
    [ObservableProperty]
    public partial int CurrentMileage { get; set; } = 125000;

    // Список записей о заменах. 
    // ObservableCollection сама говорит экрану, когда в список что-то добавили или удалили!
    public ObservableCollection<ServiceRecord> Records { get; } = new();

    // Общая сумма, потраченная на все ремонты (считается через LINQ!)
    public decimal TotalSpent => Records.Sum(r => r.TotalCost);

    public MainViewModel()
    {
        // Добавим две пробные записи, чтобы сразу увидеть их на экране:
        Records.Add(new ServiceRecord
        {
            PartTitle = "Моторное масло 5W-40 + Фильтр",
            Category = "ТО",
            Date = DateTime.Today.AddDays(-14), // 2 недели назад
            Mileage = 120000,
            PartCost = 3800m,
            LaborCost = 1200m,
            ServiceName = "СТО Вираж",
            Notes = "Артикул фильтра MANN W914/2"
        });

        Records.Add(new ServiceRecord
        {
            PartTitle = "Передние тормозные колодки",
            Category = "Тормоза",
            Date = DateTime.Today.AddMonths(-2), // 2 месяца назад
            Mileage = 115000,
            PartCost = 2400m,
            LaborCost = 0m,
            ServiceName = "Сам в гараже",
            Notes = "Заменил за 1.5 часа"
        });
    }
}