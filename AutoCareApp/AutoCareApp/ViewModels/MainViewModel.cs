using System;
using System.Collections.ObjectModel;
using System.Linq;
using AutoCareApp.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoCareApp.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    // Название автомобиля
    [ObservableProperty]
    public partial string CarName { get; set; } = "Lada Vesta 1.6";

    // Текущий пробег автомобиля
    [ObservableProperty]
    public partial int CurrentMileage { get; set; } = 125000;

    // Список записей о заменах
    public ObservableCollection<ServiceRecord> Records { get; } = new();

    // Общая сумма, потраченная на все ремонты
    public decimal TotalSpent => Records.Sum(r => r.TotalCost);

    // Флаг: открыта ли форма добавления новой записи
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AddButtonText))]
    public partial bool IsAddingRecord { get; set; } = false;

    // Текст на кнопке переключения формы
    public string AddButtonText => IsAddingRecord ? "✕ Отмена" : "+ Добавить запись о замене";

    // Поля формы ввода:
    [ObservableProperty]
    public partial string NewPartTitle { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string NewCategory { get; set; } = "ТО";

    [ObservableProperty]
    public partial decimal? NewMileage { get; set; } = 125000;

    [ObservableProperty]
    public partial decimal? NewPartCost { get; set; } = 0;

    [ObservableProperty]
    public partial decimal? NewLaborCost { get; set; } = 0;

    [ObservableProperty]
    public partial string NewServiceName { get; set; } = "Сам в гараже";

    [ObservableProperty]
    public partial string NewNotes { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string ErrorMessage { get; set; } = string.Empty;

    public MainViewModel()
    {
        // Слушаем изменения коллекции, чтобы при добавлении/удалении пересчитывать TotalSpent
        Records.CollectionChanged += (_, _) => OnPropertyChanged(nameof(TotalSpent));

        // Демо-данные
        Records.Add(new ServiceRecord
        {
            PartTitle = "Моторное масло 5W-40 + Фильтр",
            Category = "ТО",
            Date = DateTime.Today.AddDays(-14),
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
            Date = DateTime.Today.AddMonths(-2),
            Mileage = 115000,
            PartCost = 2400m,
            LaborCost = 0m,
            ServiceName = "Сам в гараже",
            Notes = "Заменил за 1.5 часа"
        });
    }

    // Команда открытия/закрытия формы
    [RelayCommand]
    private void ToggleAddForm()
    {
        IsAddingRecord = !IsAddingRecord;
        ErrorMessage = string.Empty;

        if (IsAddingRecord)
        {
            // Подставим актуальный пробег машины в поле ввода
            NewMileage = CurrentMileage;
        }
    }

    // Команда сохранения новой записи
    [RelayCommand]
    private void SaveRecord()
    {
        // 1. Валидация ввода (защита от пустых значений)
        if (string.IsNullOrWhiteSpace(NewPartTitle))
        {
            ErrorMessage = "⚠️ Пожалуйста, введите название детали!";
            return;
        }

        int mileageAtSwap = (int)(NewMileage ?? CurrentMileage);
        decimal partCost = NewPartCost ?? 0m;
        decimal laborCost = NewLaborCost ?? 0m;

        // 2. Создаем новую запись
        var record = new ServiceRecord
        {
            PartTitle = NewPartTitle.Trim(),
            Category = string.IsNullOrWhiteSpace(NewCategory) ? "ТО" : NewCategory.Trim(),
            Mileage = mileageAtSwap,
            PartCost = partCost,
            LaborCost = laborCost,
            ServiceName = string.IsNullOrWhiteSpace(NewServiceName) ? "Сам в гараже" : NewServiceName.Trim(),
            Notes = NewNotes.Trim(),
            Date = DateTime.Today
        };

        // 3. Вставляем запись в самое начало списка (чтобы свежая была сверху!)
        Records.Insert(0, record);

        // 4. Если введённый пробег больше текущего — обновим текущий пробег авто
        if (mileageAtSwap > CurrentMileage)
        {
            CurrentMileage = mileageAtSwap;
        }

        // 5. Очищаем поля формы для следующего раза
        NewPartTitle = string.Empty;
        NewPartCost = 0m;
        NewLaborCost = 0m;
        NewNotes = string.Empty;
        ErrorMessage = string.Empty;

        // Закрываем форму
        IsAddingRecord = false;
    }
}
