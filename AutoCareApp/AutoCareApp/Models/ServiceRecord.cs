using System;
using System.Globalization;
using SQLite;

namespace AutoCareApp.Models;

public class ServiceRecord
{
    // Уникальный идентификатор
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    // Внешний ключ: к какому автомобилю относится эта запись
    [Indexed]
    public int VehicleId { get; set; }

    // Что меняли
    public string PartTitle { get; set; } = string.Empty;

    // Категория (ТО, Двигатель, Подвеска, Тормоза и т.д.)
    public string Category { get; set; } = "ТО";

    // Дата проведения работ
    public DateTime Date { get; set; } = DateTime.Today;

    // Пробег автомобиля на момент замены
    public int Mileage { get; set; }

    // Стоимость детали
    public decimal PartCost { get; set; }

    // Стоимость работы
    public decimal LaborCost { get; set; }

    // Где проводилась замена
    public string ServiceName { get; set; } = "Сам в гараже";

    // Заметки
    public string Notes { get; set; } = string.Empty;

    // Свойство только для чтения: автоматически считает общую сумму
    [Ignore]
    public decimal TotalCost => PartCost + LaborCost;

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
