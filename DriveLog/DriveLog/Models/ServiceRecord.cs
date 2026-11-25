using System;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;
using System.Collections.Generic;
using System.Text.Json;

namespace DriveLog.Models;

public partial class ServiceRecord : ObservableObject
{
    // Уникальный идентификатор
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    // Внешний ключ: к какому автомобилю относится эта запись
    [Indexed]
    public int VehicleId { get; set; }

    // Что меняли
    [ObservableProperty]
    public partial string PartTitle { get; set; } = string.Empty;

    // Категория (ТО, Двигатель, Подвеска, Тормоза и т.д.)
    [ObservableProperty]
    public partial string Category { get; set; } = "ТО";

    // Дата проведения работ
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplayDate))]
    public partial DateTime Date { get; set; } = DateTime.Today;

    // Пробег автомобиля на момент замены
    [ObservableProperty]
    public partial int Mileage { get; set; }

    // Стоимость детали
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TotalCost))]
    public partial decimal PartCost { get; set; }

    // Стоимость работы
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TotalCost))]
    public partial decimal LaborCost { get; set; }

    // Где проводилась замена
    [ObservableProperty]
    public partial string ServiceName { get; set; } = "Сам в гараже";

    // Заметки
    [NotifyPropertyChangedFor(nameof(HasNotes))]
    [ObservableProperty]
    public partial string Notes { get; set; } = string.Empty;

    [Ignore]
    public bool HasNotes => !string.IsNullOrWhiteSpace(Notes);

    // Свойство только для чтения: автоматически считает общую сумму
    [Ignore]
    public decimal TotalCost => PartCost + LaborCost;


    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PartsList))]
    public partial string PartsJson { get; set; } = string.Empty;


    [Ignore]
    public List<PartItem> PartsList
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(PartsJson))
            {
                try
                {
                    var list = JsonSerializer.Deserialize<List<PartItem>>(PartsJson);
                    if (list != null && list.Count > 0) return list;
                }
                catch { }
            }
            // Запасной вариант для старых записей, где JSON ещё пустой:
            return new List<PartItem>
        {
            new PartItem { Title = PartTitle, Cost = PartCost }
        };
        }
    }


    // Умное отображение даты:
    // Если в текущем году — показывает число и месяц (например: "1 ноября")
    // Если в предыдущих годах — добавляет год (например: "15 сентября 2024 г.")
    [Ignore]
    public string DisplayDate
    {
        get
        {
            var culture = new CultureInfo("ru-RU");
            if (Date.Year == DateTime.Today.Year)
            {
                return Date.ToString("d MMMM", culture);
            }
            return Date.ToString("d MMMM yyyy", culture) + " г.";
        }
    }
}
