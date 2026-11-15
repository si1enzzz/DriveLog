using System;

namespace AutoCareApp.Models;

public class ServiceRecord
{
    public int Id { get; set; }

    // Что меняли (например: "Моторное масло 5W-40")
    public string PartTitle { get; set; } = string.Empty;

    // Категория (ТО, Двигатель, Подвеска, Тормоза и т.д.)
    public string Category { get; set; } = "ТО";

    // Дата проведения работ
    public DateTime Date { get; set; } = DateTime.Today;

    // Пробег автомобиля на момент замены
    public int Mileage { get; set; }

    // Стоимость самой детали
    public decimal PartCost { get; set; }

    // Стоимость работы мастера (0, если делал сам в гараже)
    public decimal LaborCost { get; set; }

    // Где проводилась замена ("СТО Вираж" или "Сам в гараже")
    public string ServiceName { get; set; } = "Не указано";

    // Заметки: артикул, вязкость, бренд
    public string Notes { get; set; } = string.Empty;

    // Свойство только для чтения: автоматически считает общую сумму
    public decimal TotalCost => PartCost + LaborCost;
}