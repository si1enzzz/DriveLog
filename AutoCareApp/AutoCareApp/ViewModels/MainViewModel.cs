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
    // Список марок для автоподстановки
    public IReadOnlyList<string> BrandSuggestions => CarCatalogService.PopularBrands;

    // Список всех автомобилей
    public ObservableCollection<Vehicle> Vehicles { get; } = new();

    // Текущий выбранный автомобиль
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanDeleteVehicle))]
    public partial Vehicle? SelectedVehicle { get; set; }

    // Можно ли удалить машину (если осталась только 1 — нельзя)
    public bool CanDeleteVehicle => Vehicles.Count > 1;

    // --- ФОРМА ДОБАВЛЕНИЯ АВТОМОБИЛЯ ---
    [ObservableProperty]
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
    public partial bool IsAddingRecord { get; set; } = false;

    public string AddButtonText => IsAddingRecord ? "Отмена" : "Добавить запись о замене";

    [ObservableProperty]
    public partial string NewPartTitle { get; set; } = string.Empty;

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
        // Создаем стартовый автомобиль с подробными параметрами
        var defaultCar = new Vehicle
        {
            Brand = "Lada",
            Model = "Vesta",
            Generation = "I",
            Year = 2018,
            EngineVolume = "1.6",
            Color = "Тёмно-серый",
            CurrentMileage = 125000
        };

        defaultCar.Records.Add(new ServiceRecord
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

        defaultCar.Records.Add(new ServiceRecord
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

        Vehicles.Add(defaultCar);
        SelectedVehicle = defaultCar;
    }

    partial void OnSelectedVehicleChanged(Vehicle? value)
    {
        OnPropertyChanged(nameof(CanDeleteVehicle));
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
        VehicleErrorMessage = string.Empty;
        NewBrand = string.Empty;
        NewModel = string.Empty;
        NewGeneration = string.Empty;
        NewYear = DateTime.Today.Year;
        NewEngineVolume = string.Empty;
        NewColor = string.Empty;
        NewVehicleMileage = 0;
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

        Vehicles.Add(vehicle);
        SelectedVehicle = vehicle;

        VehicleErrorMessage = string.Empty;
        IsAddingVehicle = false;
        OnPropertyChanged(nameof(CanDeleteVehicle));
    }

    [RelayCommand]
    private void DeleteCurrentVehicle()
    {
        if (SelectedVehicle == null || Vehicles.Count <= 1)
        {
            return;
        }

        var toRemove = SelectedVehicle;
        Vehicles.Remove(toRemove);
        SelectedVehicle = Vehicles.FirstOrDefault();
        OnPropertyChanged(nameof(CanDeleteVehicle));
    }

    // --- УПРАВЛЕНИЕ ЗАПИСЯМИ ---

    [RelayCommand]
    private void ToggleAddForm()
    {
        IsAddingRecord = !IsAddingRecord;
        RecordErrorMessage = string.Empty;

        if (IsAddingRecord && SelectedVehicle != null)
        {
            NewMileage = SelectedVehicle.CurrentMileage;
        }
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
            PartTitle = NewPartTitle.Trim(),
            Category = string.IsNullOrWhiteSpace(NewCategory) ? "ТО" : NewCategory.Trim(),
            Mileage = mileageAtSwap,
            PartCost = partCost,
            LaborCost = laborCost,
            ServiceName = string.IsNullOrWhiteSpace(NewServiceName) ? "Сам в гараже" : NewServiceName.Trim(),
            Notes = NewNotes.Trim(),
            Date = DateTime.Today
        };

        SelectedVehicle.Records.Insert(0, record);

        if (mileageAtSwap > SelectedVehicle.CurrentMileage)
        {
            SelectedVehicle.CurrentMileage = mileageAtSwap;
        }

        NewPartTitle = string.Empty;
        NewPartCost = 0m;
        NewLaborCost = 0m;
        NewNotes = string.Empty;
        RecordErrorMessage = string.Empty;
        IsAddingRecord = false;
    }
}
