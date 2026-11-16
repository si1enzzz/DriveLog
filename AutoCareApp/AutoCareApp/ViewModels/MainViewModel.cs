using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AutoCareApp.Models;
using AutoCareApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AutoCareApp.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    // Сервис базы данных SQLite
    private readonly DatabaseService _db = DatabaseService.Instance;

    // Список марок для автоподстановки
    public IReadOnlyList<string> BrandSuggestions => CarCatalogService.PopularBrands;

    // Динамические списки моделей и поколений (обновляются при вводе марки)
    public ObservableCollection<string> ModelSuggestions { get; } = new();
    public ObservableCollection<string> GenerationSuggestions { get; } = new();

    // Список всех автомобилей
    public ObservableCollection<Vehicle> Vehicles { get; } = new();

    // Текущий выбранный автомобиль
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanDeleteVehicle))]
    public partial Vehicle? SelectedVehicle { get; set; }

    // Можно ли удалить машину (если выбрана любая машина — можно удалить)
    public bool CanDeleteVehicle => SelectedVehicle != null;

    // Есть ли в гараже хотя бы один автомобиль
    public bool HasVehicles => Vehicles.Count > 0;

    // --- ФОРМА ДОБАВЛЕНИЯ АВТОМОБИЛЯ ---
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsAnyFormOpen))]
    public partial bool IsAddingVehicle { get; set; } = false;

    [ObservableProperty]
    public partial string NewBrand { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string NewModel { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string NewGeneration { get; set; } = string.Empty;

    [ObservableProperty]
    public partial decimal? NewYear { get; set; } = DateTime.Today.Year;

    [ObservableProperty]
    public partial string NewEngineVolume { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string NewColor { get; set; } = string.Empty;

    [ObservableProperty]
    public partial decimal? NewVehicleMileage { get; set; } = 0;

    [ObservableProperty]
    public partial string VehicleErrorMessage { get; set; } = string.Empty;

    // --- ФОРМА ДОБАВЛЕНИЯ ЗАПИСИ ---
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AddButtonText))]
    [NotifyPropertyChangedFor(nameof(IsAnyFormOpen))]
    public partial bool IsAddingRecord { get; set; } = false;

    public string AddButtonText => IsAddingRecord ? "Отмена" : "Добавить запись о замене";

    // Открыта ли хоть одна модальная форма (для эффекта затемнения фона)
    public bool IsAnyFormOpen => IsAddingVehicle || IsAddingRecord;

    [ObservableProperty]
    public partial string NewPartTitle { get; set; } = string.Empty;

    [ObservableProperty]
    public partial DateTimeOffset? NewDate { get; set; } = DateTimeOffset.Now;

    [ObservableProperty]
    public partial string NewCategory { get; set; } = "ТО";

    [ObservableProperty]
    public partial decimal? NewMileage { get; set; } = 0;

    [ObservableProperty]
    public partial decimal? NewPartCost { get; set; } = 0;

    [ObservableProperty]
    public partial decimal? NewLaborCost { get; set; } = 0;

    [ObservableProperty]
    public partial string NewServiceName { get; set; } = "Сам в гараже";

    [ObservableProperty]
    public partial string NewNotes { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string RecordErrorMessage { get; set; } = string.Empty;

    public MainViewModel()
    {
        // Загрузка сохранённых автомобилей из локальной базы данных SQLite
        var savedVehicles = _db.GetAllVehicles();
        foreach (var v in savedVehicles)
        {
            Vehicles.Add(v);
        }
        SelectedVehicle = Vehicles.FirstOrDefault();
    }

    // Реакция на изменение марки: обновляем доступные модели
    partial void OnNewBrandChanged(string value)
    {
        ModelSuggestions.Clear();
        GenerationSuggestions.Clear();

        var models = CarCatalogService.GetModelsForBrand(value);
        foreach (var m in models)
        {
            ModelSuggestions.Add(m);
        }
    }

    // Реакция на изменение модели: обновляем поколения
    partial void OnNewModelChanged(string value)
    {
        GenerationSuggestions.Clear();

        var gens = CarCatalogService.GetGenerationsForModel(NewBrand, value);
        foreach (var g in gens)
        {
            GenerationSuggestions.Add(g);
        }
    }

    // Реакция на выбор/ввод поколения: автоматически выставляем минимальный (начальный) год этого поколения
    partial void OnNewGenerationChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return;

        // Ищем 4-значный год перед дефисом/тире: например "2018" из "IV (BD, 2018–2021)" или "2023" из "(2023–н.в.)"
        var match = System.Text.RegularExpressions.Regex.Match(value, @"\b(19\d{2}|20\d{2})\s*[-–—]");
        if (match.Success && int.TryParse(match.Groups[1].Value, out int year))
        {
            NewYear = year;
            return;
        }

        // Запасной вариант: любой 4-значный год в диапазоне 1950-2035
        var fallback = System.Text.RegularExpressions.Regex.Match(value, @"\b(19[5-9]\d|20[0-3]\d)\b");
        if (fallback.Success && int.TryParse(fallback.Value, out int fallbackYear))
        {
            NewYear = fallbackYear;
        }
    }

    partial void OnSelectedVehicleChanged(Vehicle? value)
    {
        OnPropertyChanged(nameof(CanDeleteVehicle));
        OnPropertyChanged(nameof(HasVehicles));
        if (value != null)
        {
            NewMileage = value.CurrentMileage;
        }
    }

    // --- УПРАВЛЕНИЕ АВТОМОБИЛЯМИ ---

    [RelayCommand]
    private void ToggleAddVehicle()
    {
        IsAddingVehicle = !IsAddingVehicle;
        if (IsAddingVehicle)
        {
            // Взаимное закрытие: если открываем добавление авто, закрываем форму записи
            IsAddingRecord = false;
        }

        VehicleErrorMessage = string.Empty;
        NewBrand = string.Empty;
        NewModel = string.Empty;
        NewGeneration = string.Empty;
        NewYear = DateTime.Today.Year;
        NewEngineVolume = string.Empty;
        NewColor = string.Empty;
        NewVehicleMileage = 0;
        ModelSuggestions.Clear();
        GenerationSuggestions.Clear();
    }

    [RelayCommand]
    private void AddVehicle()
    {
        if (string.IsNullOrWhiteSpace(NewBrand))
        {
            VehicleErrorMessage = "Укажите марку автомобиля!";
            return;
        }

        if (string.IsNullOrWhiteSpace(NewModel))
        {
            VehicleErrorMessage = "Укажите модель автомобиля!";
            return;
        }

        var vehicle = new Vehicle
        {
            Brand = NewBrand.Trim(),
            Model = NewModel.Trim(),
            Generation = NewGeneration.Trim(),
            Year = (int)(NewYear ?? DateTime.Today.Year),
            EngineVolume = NewEngineVolume.Trim(),
            Color = NewColor.Trim(),
            CurrentMileage = (int)(NewVehicleMileage ?? 0)
        };

        // Сохранение в базу данных SQLite (присваивает уникальный Id)
        _db.SaveVehicle(vehicle);

        Vehicles.Add(vehicle);
        SelectedVehicle = vehicle;

        VehicleErrorMessage = string.Empty;
        IsAddingVehicle = false;
        OnPropertyChanged(nameof(CanDeleteVehicle));
        OnPropertyChanged(nameof(HasVehicles));
    }

    [RelayCommand]
    private void DeleteCurrentVehicle()
    {
        if (SelectedVehicle == null)
        {
            return;
        }

        var toRemove = SelectedVehicle;
        _db.DeleteVehicle(toRemove.Id);
        Vehicles.Remove(toRemove);
        SelectedVehicle = Vehicles.FirstOrDefault();
        OnPropertyChanged(nameof(CanDeleteVehicle));
        OnPropertyChanged(nameof(HasVehicles));
    }

    // --- УПРАВЛЕНИЕ ЗАПИСЯМИ ---

    [RelayCommand]
    private void ToggleAddForm()
    {
        IsAddingRecord = !IsAddingRecord;
        if (IsAddingRecord)
        {
            // Взаимное закрытие: если открываем добавление записи, закрываем добавление авто
            IsAddingVehicle = false;
        }

        RecordErrorMessage = string.Empty;

        if (IsAddingRecord && SelectedVehicle != null)
        {
            NewMileage = SelectedVehicle.CurrentMileage;
            NewDate = DateTimeOffset.Now;
        }
    }

    [RelayCommand]
    private void CloseAllForms()
    {
        IsAddingVehicle = false;
        IsAddingRecord = false;
        VehicleErrorMessage = string.Empty;
        RecordErrorMessage = string.Empty;
    }

    [RelayCommand]
    private void SaveRecord()
    {
        if (SelectedVehicle == null)
        {
            RecordErrorMessage = "Сначала выберите автомобиль!";
            return;
        }

        if (string.IsNullOrWhiteSpace(NewPartTitle))
        {
            RecordErrorMessage = "Введите название детали!";
            return;
        }

        int mileageAtSwap = (int)(NewMileage ?? SelectedVehicle.CurrentMileage);
        decimal partCost = NewPartCost ?? 0m;
        decimal laborCost = NewLaborCost ?? 0m;

        var record = new ServiceRecord
        {
            VehicleId = SelectedVehicle.Id,
            PartTitle = NewPartTitle.Trim(),
            Category = string.IsNullOrWhiteSpace(NewCategory) ? "ТО" : NewCategory.Trim(),
            Mileage = mileageAtSwap,
            PartCost = partCost,
            LaborCost = laborCost,
            ServiceName = string.IsNullOrWhiteSpace(NewServiceName) ? "Сам в гараже" : NewServiceName.Trim(),
            Notes = NewNotes.Trim(),
            Date = (NewDate ?? DateTimeOffset.Now).DateTime
        };

        // Сохраняем запись в базу данных SQLite
        _db.SaveRecord(record);

        SelectedVehicle.Records.Insert(0, record);

        if (mileageAtSwap > SelectedVehicle.CurrentMileage)
        {
            SelectedVehicle.CurrentMileage = mileageAtSwap;
            _db.SaveVehicle(SelectedVehicle); // сохраняем обновленный пробег машины
        }

        NewPartTitle = string.Empty;
        NewPartCost = 0m;
        NewLaborCost = 0m;
        NewNotes = string.Empty;
        NewDate = DateTimeOffset.Now;
        RecordErrorMessage = string.Empty;
        IsAddingRecord = false;
    }
}
