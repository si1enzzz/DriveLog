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
    public ObservableCollection<string> BrandSuggestions { get; } = new(CarCatalogService.PopularBrands);

    // Динамические списки моделей, поколений и двигателей (обновляются при вводе марки и модели)
    public ObservableCollection<string> ModelSuggestions { get; } = new();
    public ObservableCollection<string> GenerationSuggestions { get; } = new();
        public ObservableCollection<string> EngineSuggestions { get; } = new();
    public ObservableCollection<string> HorsepowerSuggestions { get; } = new()
    {
        "75", "80", "85", "90", "98", "100", "105", "106", "110", "115", "120", "122", "123", "125",
        "130", "140", "145", "149", "150", "160", "170", "180", "190", "200", "220", "240", "249", "250", "300"
    };
    public ObservableCollection<string> ColorSuggestions { get; } = new()
    {
        "Белый", "Черный", "Серый", "Серебристый", "Синий", "Красный", "Зеленый", 
        "Коричневый", "Бежевый", "Голубой", "Оранжевый", "Желтый", "Бордовый", "Фиолетовый", "Золотистый"
    };

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

    // --- НАВИГАЦИЯ МЕЖДУ ВКЛАДКАМИ (TELEGRAM FLOATING PILL BAR) ---
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsGarageTab))]
    [NotifyPropertyChangedFor(nameof(IsAccountTab))]
    [NotifyPropertyChangedFor(nameof(GarageTabPillColor))]
    [NotifyPropertyChangedFor(nameof(AccountTabPillColor))]
    [NotifyPropertyChangedFor(nameof(GarageTabColor))]
    [NotifyPropertyChangedFor(nameof(AccountTabColor))]
    [NotifyPropertyChangedFor(nameof(CanShowFab))]
    public partial int SelectedTabIndex { get; set; } = 0; // 0 = Гараж, 1 = Аккаунт

    public bool IsGarageTab => SelectedTabIndex == 0;
    public bool IsAccountTab => SelectedTabIndex == 1;
    public bool CanShowFab => IsGarageTab && HasVehicles;

    // Telegram-style floating pill colors
    public string GarageTabPillColor => IsGarageTab 
        ? (IsDarkTheme ? "#162846" : "#D8E5F8") 
        : "Transparent";
    public string AccountTabPillColor => IsAccountTab 
        ? (IsDarkTheme ? "#162846" : "#D8E5F8") 
        : "Transparent";

    public string GarageTabColor => IsGarageTab ? "#074CBA" : (IsDarkTheme ? "#8E8E93" : "#6C6C70");
    public string AccountTabColor => IsAccountTab ? "#074CBA" : (IsDarkTheme ? "#8E8E93" : "#6C6C70");

    [RelayCommand]
    private void SelectGarageTab()
    {
        SelectedTabIndex = 0;
        RefreshTabBindings();
    }

    [RelayCommand]
    private void SelectAccountTab()
    {
        SelectedTabIndex = 1;
        RefreshTabBindings();
    }

    private void RefreshTabBindings()
    {
        OnPropertyChanged(nameof(IsGarageTab));
        OnPropertyChanged(nameof(IsAccountTab));
        OnPropertyChanged(nameof(GarageTabPillColor));
        OnPropertyChanged(nameof(AccountTabPillColor));
        OnPropertyChanged(nameof(GarageTabColor));
        OnPropertyChanged(nameof(AccountTabColor));
        OnPropertyChanged(nameof(CanShowFab));
    }

    // --- ТЕМА ОФОРМЛЕНИЯ (DARK / LIGHT) ---
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(BgColor))]
    [NotifyPropertyChangedFor(nameof(CardColor))]
    [NotifyPropertyChangedFor(nameof(CardBorderColor))]
    [NotifyPropertyChangedFor(nameof(InputBgColor))]
    [NotifyPropertyChangedFor(nameof(InputBorderColor))]
    [NotifyPropertyChangedFor(nameof(AccentColor))]
    [NotifyPropertyChangedFor(nameof(TextPrimaryColor))]
    [NotifyPropertyChangedFor(nameof(TextSecondaryColor))]
    [NotifyPropertyChangedFor(nameof(TextHighlightColor))]
    [NotifyPropertyChangedFor(nameof(BottomBarBgColor))]
    [NotifyPropertyChangedFor(nameof(BottomBarBorderColor))]
    [NotifyPropertyChangedFor(nameof(GarageTabPillColor))]
    [NotifyPropertyChangedFor(nameof(AccountTabPillColor))]
    [NotifyPropertyChangedFor(nameof(GarageTabColor))]
    [NotifyPropertyChangedFor(nameof(AccountTabColor))]
    [NotifyPropertyChangedFor(nameof(DarkThemeBtnBg))]
    [NotifyPropertyChangedFor(nameof(DarkThemeBtnFg))]
    [NotifyPropertyChangedFor(nameof(LightThemeBtnBg))]
    [NotifyPropertyChangedFor(nameof(LightThemeBtnFg))]
    [NotifyPropertyChangedFor(nameof(RuLangBtnBg))]
    [NotifyPropertyChangedFor(nameof(RuLangBtnFg))]
    [NotifyPropertyChangedFor(nameof(EnLangBtnBg))]
    [NotifyPropertyChangedFor(nameof(EnLangBtnFg))]
    public partial bool IsDarkTheme { get; set; } = true;

    // Палитра Dark: фон #000000, карточки #0F0F0F, акцент #074CBA, расходы #F2F2F2
    // Палитра Light: фон #E3E3E3, карточки #FFFFFF, акцент #074CBA, расходы #0F0F0F
    public string BgColor => IsDarkTheme ? "#000000" : "#E3E3E3";
    public string CardColor => IsDarkTheme ? "#0F0F0F" : "#FFFFFF";
    public string CardBorderColor => IsDarkTheme ? "#1C1C1E" : "#D1D1D6";
    public string InputBgColor => IsDarkTheme ? "#161618" : "#F2F2F7";
    public string InputBorderColor => IsDarkTheme ? "#2C2C30" : "#C7C7CC";
    public string AccentColor => "#074CBA";
    public string TextPrimaryColor => IsDarkTheme ? "#FFFFFF" : "#000000";
    public string TextSecondaryColor => IsDarkTheme ? "#8E8E93" : "#6C6C70";
    public string TextHighlightColor => IsDarkTheme ? "#F2F2F2" : "#0F0F0F";

    public string BottomBarBgColor => IsDarkTheme ? "#141416" : "#FFFFFF";
    public string BottomBarBorderColor => IsDarkTheme ? "#222226" : "#D1D1D6";

    public string DarkThemeBtnBg => IsDarkTheme ? "#074CBA" : (IsDarkTheme ? "#1F1F24" : "#E2E2E6");
    public string DarkThemeBtnFg => IsDarkTheme ? "#FFFFFF" : (IsDarkTheme ? "#8E8E93" : "#6C6C70");
    public string LightThemeBtnBg => !IsDarkTheme ? "#074CBA" : (IsDarkTheme ? "#1F1F24" : "#E2E2E6");
    public string LightThemeBtnFg => !IsDarkTheme ? "#FFFFFF" : (IsDarkTheme ? "#8E8E93" : "#6C6C70");

    [RelayCommand]
    private void SetDarkTheme()
    {
        IsDarkTheme = true;
        ApplyThemeVariant();
    }

    [RelayCommand]
    private void SetLightTheme()
    {
        IsDarkTheme = false;
        ApplyThemeVariant();
    }

    private void ApplyThemeVariant()
    {
        if (Avalonia.Application.Current != null)
        {
            Avalonia.Application.Current.RequestedThemeVariant = IsDarkTheme 
                ? Avalonia.Styling.ThemeVariant.Dark 
                : Avalonia.Styling.ThemeVariant.Light;
        }
    }

    // --- ЛОКАЛИЗАЦИЯ ИНТЕРФЕЙСА (RU / EN) ---
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsRussian))]
    [NotifyPropertyChangedFor(nameof(IsEnglish))]
    [NotifyPropertyChangedFor(nameof(RuLangBtnBg))]
    [NotifyPropertyChangedFor(nameof(RuLangBtnFg))]
    [NotifyPropertyChangedFor(nameof(EnLangBtnBg))]
    [NotifyPropertyChangedFor(nameof(EnLangBtnFg))]
    [NotifyPropertyChangedFor(nameof(DeleteConfirmMessage))]
    public partial string CurrentLanguage { get; set; } = "ru";

    public bool IsRussian => CurrentLanguage == "ru";
    public bool IsEnglish => CurrentLanguage == "en";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DeleteConfirmMessage))]
    public partial AppStrings Strings { get; set; } = AppStrings.Russian;

    public string RuLangBtnBg => IsRussian ? "#074CBA" : (IsDarkTheme ? "#1F1F24" : "#E2E2E6");
    public string RuLangBtnFg => IsRussian ? "#FFFFFF" : (IsDarkTheme ? "#8E8E93" : "#6C6C70");
    public string EnLangBtnBg => IsEnglish ? "#074CBA" : (IsDarkTheme ? "#1F1F24" : "#E2E2E6");
    public string EnLangBtnFg => IsEnglish ? "#FFFFFF" : (IsDarkTheme ? "#8E8E93" : "#6C6C70");

    [RelayCommand]
    private void SetRussianLanguage()
    {
        CurrentLanguage = "ru";
        Strings = AppStrings.Russian;
        OnPropertyChanged(nameof(DeleteConfirmMessage));
    }

    [RelayCommand]
    private void SetEnglishLanguage()
    {
        CurrentLanguage = "en";
        Strings = AppStrings.English;
        OnPropertyChanged(nameof(DeleteConfirmMessage));
    }

    public string DeleteConfirmMessage => SelectedVehicle == null 
        ? "" 
        : string.Format(Strings.DeleteConfirmFormat, SelectedVehicle.Title, SelectedVehicle.Records.Count);

    // --- ДАННЫЕ ПРОФИЛЯ И ВКЛАДКИ АККАУНТА ---
    [ObservableProperty]
    public partial string DriverName { get; set; } = "Водитель";

    [ObservableProperty]
    public partial string DriverExperience { get; set; } = "2 года";

    [ObservableProperty]
    public partial string ExportStatusMessage { get; set; } = string.Empty;

    public decimal TotalGarageSpent => Vehicles.Sum(v => v.TotalSpent);
    public int TotalRecordsCount => Vehicles.Sum(v => v.Records.Count);

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
    public partial string NewHorsepower { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string NewColor { get; set; } = string.Empty;

    [ObservableProperty]
    public partial decimal? NewVehicleMileage { get; set; } = 0;

    [ObservableProperty]
    public partial string VehicleErrorMessage { get; set; } = string.Empty;

    // Режим редактирования автомобиля (null = создание нового)
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(VehicleFormTitle))]
    [NotifyPropertyChangedFor(nameof(VehicleFormSubmitButtonText))]
    public partial int? EditingVehicleId { get; set; } = null;

    public string VehicleFormTitle => EditingVehicleId != null ? Strings.EditVehicle : Strings.NewVehicle;
    public string VehicleFormSubmitButtonText => EditingVehicleId != null ? Strings.Save : Strings.Add;

    // --- ДИАЛОГ ПОДТВЕРЖДЕНИЯ УДАЛЕНИЯ АВТОМОБИЛЯ ---
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsAnyFormOpen))]
    public partial bool IsConfirmingDeleteVehicle { get; set; } = false;

    // --- ФОРМА ДОБАВЛЕНИЯ ЗАПИСИ ---
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AddButtonText))]
    [NotifyPropertyChangedFor(nameof(IsAnyFormOpen))]
    public partial bool IsAddingRecord { get; set; } = false;

    public string AddButtonText => IsAddingRecord ? "Отмена" : "Добавить запись о замене";

    // Открыта ли хоть одна модальная форма (для эффекта затемнения фона)
    public bool IsAnyFormOpen => IsAddingVehicle || IsAddingRecord || IsConfirmingDeleteVehicle;

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

    // Режим редактирования записи о замене (null = создание новой)
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(RecordFormTitle))]
    [NotifyPropertyChangedFor(nameof(RecordFormSubmitButtonText))]
    public partial int? EditingRecordId { get; set; } = null;

    public string RecordFormTitle => EditingRecordId != null ? Strings.EditRecord : Strings.NewRecord;
    public string RecordFormSubmitButtonText => EditingRecordId != null ? Strings.Save : Strings.SaveRecord;

    public MainViewModel()
    {
        // Загрузка сохранённых автомобилей из локальной базы данных SQLite
        try
        {
            var savedVehicles = _db.GetAllVehicles();
            foreach (var v in savedVehicles)
            {
                Vehicles.Add(v);
            }
            SelectedVehicle = Vehicles.FirstOrDefault();
        }
        catch { }


    }

    // Реакция на изменение марки: обновляем подсказки марок (с поддержкой русского ввода) и доступные модели
    partial void OnNewBrandChanged(string value)
    {
        var filteredBrands = CarCatalogService.FilterBrands(value);
        BrandSuggestions.Clear();
        foreach (var b in filteredBrands)
        {
            BrandSuggestions.Add(b);
        }

        ModelSuggestions.Clear();
        GenerationSuggestions.Clear();
        EngineSuggestions.Clear();

        var models = CarCatalogService.GetModelsForBrand(value);
        foreach (var m in models)
        {
            ModelSuggestions.Add(m);
        }
    }

    // Реакция на изменение модели: обновляем поколения и список возможных моторов
    partial void OnNewModelChanged(string value)
    {
        GenerationSuggestions.Clear();
        EngineSuggestions.Clear();

        var gens = CarCatalogService.GetGenerationsForModel(NewBrand, value);
        foreach (var g in gens)
        {
            GenerationSuggestions.Add(g);
        }

        var engines = CarCatalogService.GetEnginesForModel(NewBrand, value);
        foreach (var e in engines)
        {
            EngineSuggestions.Add(e.DisplayText);
        }
    }

    // Реакция на выбор/ввод двигателя: если выбран вариант с л.с. вида "1.6 (123 л.с.)", авто-заполняем мощность
    partial void OnNewEngineVolumeChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return;

        var match = System.Text.RegularExpressions.Regex.Match(value, @"\(([0-9]+)\s*л\.с\.\)");
        if (match.Success && int.TryParse(match.Groups[1].Value, out int hp))
        {
            NewHorsepower = match.Groups[1].Value;

            var volMatch = System.Text.RegularExpressions.Regex.Match(value, @"^([^\(]+)");
            if (volMatch.Success)
            {
                var cleanVol = volMatch.Groups[1].Value.Trim().TrimEnd('л').Trim();
                if (!string.IsNullOrWhiteSpace(cleanVol))
                {
                    NewEngineVolume = cleanVol;
                }
            }
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
        OnPropertyChanged(nameof(CanShowFab));
        OnPropertyChanged(nameof(DeleteConfirmMessage));
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
        EditingVehicleId = null;
        if (IsAddingVehicle)
        {
            // Взаимное закрытие: если открываем добавление авто, закрываем форму записи и окно удаления
            IsAddingRecord = false;
            IsConfirmingDeleteVehicle = false;
        }

        VehicleErrorMessage = string.Empty;
        NewBrand = string.Empty;
        NewModel = string.Empty;
        NewGeneration = string.Empty;
        NewYear = DateTime.Today.Year;
        NewEngineVolume = string.Empty;
        NewHorsepower = string.Empty;
        NewColor = string.Empty;
        NewVehicleMileage = 0;
        BrandSuggestions.Clear();
        foreach (var b in CarCatalogService.PopularBrands) BrandSuggestions.Add(b);
        ModelSuggestions.Clear();
        GenerationSuggestions.Clear();
        EngineSuggestions.Clear();
        OnPropertyChanged(nameof(VehicleFormTitle));
        OnPropertyChanged(nameof(VehicleFormSubmitButtonText));
    }

    [RelayCommand]
    private void EditCurrentVehicle()
    {
        if (SelectedVehicle == null) return;

        EditingVehicleId = SelectedVehicle.Id;
        NewBrand = SelectedVehicle.Brand;
        NewModel = SelectedVehicle.Model;
        NewGeneration = SelectedVehicle.Generation;
        NewYear = SelectedVehicle.Year;
        NewEngineVolume = SelectedVehicle.EngineVolume;
        NewHorsepower = SelectedVehicle.Horsepower > 0 ? SelectedVehicle.Horsepower.ToString() : string.Empty;
        NewColor = SelectedVehicle.Color;
        NewVehicleMileage = SelectedVehicle.CurrentMileage;
        VehicleErrorMessage = string.Empty;

        // Заполняем подсказки для текущего авто
        BrandSuggestions.Clear();
        foreach (var b in CarCatalogService.PopularBrands) BrandSuggestions.Add(b);

        ModelSuggestions.Clear();
        foreach (var m in CarCatalogService.GetModelsForBrand(NewBrand)) ModelSuggestions.Add(m);

        GenerationSuggestions.Clear();
        foreach (var g in CarCatalogService.GetGenerationsForModel(NewBrand, NewModel)) GenerationSuggestions.Add(g);

        EngineSuggestions.Clear();
        foreach (var e in CarCatalogService.GetEnginesForModel(NewBrand, NewModel)) EngineSuggestions.Add(e.DisplayText);

        IsAddingVehicle = true;
        IsAddingRecord = false;
        IsConfirmingDeleteVehicle = false;
        OnPropertyChanged(nameof(VehicleFormTitle));
        OnPropertyChanged(nameof(VehicleFormSubmitButtonText));
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

        var canonicalBrand = CarCatalogService.ResolveCanonicalBrand(NewBrand);
        var brandToSave = string.IsNullOrWhiteSpace(canonicalBrand) ? NewBrand.Trim() : canonicalBrand;

        if (EditingVehicleId != null)
        {
            // Режим редактирования существующей машины
            var v = Vehicles.FirstOrDefault(x => x.Id == EditingVehicleId.Value);
            if (v != null)
            {
                v.Brand = brandToSave;
                v.Model = NewModel.Trim();
                v.Generation = NewGeneration?.Trim() ?? string.Empty;
                v.Year = (int)(NewYear ?? DateTime.Today.Year);
                v.EngineVolume = NewEngineVolume?.Trim() ?? string.Empty;
                int.TryParse(NewHorsepower?.Trim(), out int parsedHp); v.Horsepower = parsedHp;
                v.Color = NewColor?.Trim() ?? string.Empty;
                v.CurrentMileage = (int)(NewVehicleMileage ?? 0);

                _db.SaveVehicle(v);

                OnPropertyChanged(nameof(SelectedVehicle));
            }
        }
        else
        {
            // Режим создания новой машины
            var vehicle = new Vehicle
            {
                Brand = brandToSave,
                Model = NewModel.Trim(),
                Generation = NewGeneration?.Trim() ?? string.Empty,
                Year = (int)(NewYear ?? DateTime.Today.Year),
                EngineVolume = NewEngineVolume?.Trim() ?? string.Empty,
                Horsepower = int.TryParse(NewHorsepower?.Trim(), out int pAddHp) ? pAddHp : 0,
                Color = NewColor?.Trim() ?? string.Empty,
                CurrentMileage = (int)(NewVehicleMileage ?? 0)
            };

            // Сохранение в базу данных SQLite (присваивает уникальный Id)
            _db.SaveVehicle(vehicle);

            Vehicles.Add(vehicle);
            SelectedVehicle = vehicle;
        }

        EditingVehicleId = null;
        VehicleErrorMessage = string.Empty;
        IsAddingVehicle = false;
        OnPropertyChanged(nameof(CanDeleteVehicle));
        OnPropertyChanged(nameof(HasVehicles));
        OnPropertyChanged(nameof(CanShowFab));
        OnPropertyChanged(nameof(DeleteConfirmMessage));
        OnPropertyChanged(nameof(TotalGarageSpent));
        OnPropertyChanged(nameof(TotalRecordsCount));
    }

    // Запрос на удаление (открывает окно подтверждения)
    [RelayCommand]
    private void RequestDeleteVehicle()
    {
        if (SelectedVehicle == null) return;

        IsAddingVehicle = false;
        IsAddingRecord = false;
        IsConfirmingDeleteVehicle = true;
    }

    // Отмена удаления в модальном окне
    [RelayCommand]
    private void CancelDeleteVehicle()
    {
        IsConfirmingDeleteVehicle = false;
    }

    // Реальное удаление после подтверждения пользователем
    [RelayCommand]
    private void ConfirmDeleteVehicle()
    {
        if (SelectedVehicle == null)
        {
            IsConfirmingDeleteVehicle = false;
            return;
        }

        var toRemove = SelectedVehicle;
        _db.DeleteVehicle(toRemove.Id);
        Vehicles.Remove(toRemove);
        SelectedVehicle = Vehicles.FirstOrDefault();
        IsConfirmingDeleteVehicle = false;
        OnPropertyChanged(nameof(CanDeleteVehicle));
        OnPropertyChanged(nameof(HasVehicles));
        OnPropertyChanged(nameof(CanShowFab));
        OnPropertyChanged(nameof(DeleteConfirmMessage));
        OnPropertyChanged(nameof(TotalGarageSpent));
        OnPropertyChanged(nameof(TotalRecordsCount));
    }

    // --- УПРАВЛЕНИЕ ЗАПИСЯМИ ---

    [RelayCommand]
    private void ToggleAddForm()
    {
        IsAddingRecord = !IsAddingRecord;
        EditingRecordId = null;
        if (IsAddingRecord)
        {
            // Взаимное закрытие: если открываем добавление записи, закрываем добавление авто и окно удаления
            IsAddingVehicle = false;
            IsConfirmingDeleteVehicle = false;
            NewPartTitle = string.Empty;
            NewCategory = "ТО";
            NewPartCost = 0m;
            NewLaborCost = 0m;
            NewNotes = string.Empty;
            NewServiceName = "Сам в гараже";
        }

        RecordErrorMessage = string.Empty;

        if (IsAddingRecord && SelectedVehicle != null)
        {
            NewMileage = SelectedVehicle.CurrentMileage;
            NewDate = DateTimeOffset.Now;
        }

        OnPropertyChanged(nameof(RecordFormTitle));
        OnPropertyChanged(nameof(RecordFormSubmitButtonText));
    }

    [RelayCommand]
    private void EditRecord(ServiceRecord? record)
    {
        if (record == null) return;

        EditingRecordId = record.Id;
        NewPartTitle = record.PartTitle;
        NewCategory = record.Category;
        NewDate = new DateTimeOffset(record.Date);
        NewMileage = record.Mileage;
        NewPartCost = record.PartCost;
        NewLaborCost = record.LaborCost;
        NewServiceName = record.ServiceName;
        NewNotes = record.Notes;
        RecordErrorMessage = string.Empty;

        IsAddingRecord = true;
        IsAddingVehicle = false;
        IsConfirmingDeleteVehicle = false;

        OnPropertyChanged(nameof(RecordFormTitle));
        OnPropertyChanged(nameof(RecordFormSubmitButtonText));
    }

    [RelayCommand]
    private void DeleteRecord(ServiceRecord? record)
    {
        if (record == null || SelectedVehicle == null) return;

        _db.DeleteRecord(record.Id);
        SelectedVehicle.Records.Remove(record);
        SelectedVehicle.NotifyTotalSpentChanged();

        OnPropertyChanged(nameof(TotalGarageSpent));
        OnPropertyChanged(nameof(TotalRecordsCount));
    }

    [RelayCommand]
    private void CloseAllForms()
    {
        IsAddingVehicle = false;
        IsAddingRecord = false;
        IsConfirmingDeleteVehicle = false;
        EditingVehicleId = null;
        EditingRecordId = null;
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

        if (EditingRecordId != null)
        {
            // Режим редактирования существующей записи
            var existing = SelectedVehicle.Records.FirstOrDefault(r => r.Id == EditingRecordId.Value);
            if (existing != null)
            {
                existing.PartTitle = NewPartTitle.Trim();
                existing.Category = string.IsNullOrWhiteSpace(NewCategory) ? "ТО" : NewCategory.Trim();
                existing.Mileage = mileageAtSwap;
                existing.PartCost = partCost;
                existing.LaborCost = laborCost;
                existing.ServiceName = string.IsNullOrWhiteSpace(NewServiceName) ? "Сам в гараже" : NewServiceName.Trim();
                existing.Notes = NewNotes?.Trim() ?? string.Empty;
                existing.Date = (NewDate ?? DateTimeOffset.Now).DateTime;

                _db.SaveRecord(existing);

                if (mileageAtSwap > SelectedVehicle.CurrentMileage)
                {
                    SelectedVehicle.CurrentMileage = mileageAtSwap;
                    _db.SaveVehicle(SelectedVehicle);
                }

                SelectedVehicle.NotifyTotalSpentChanged();
            }
        }
        else
        {
            // Режим создания новой записи
            var record = new ServiceRecord
            {
                VehicleId = SelectedVehicle.Id,
                PartTitle = NewPartTitle.Trim(),
                Category = string.IsNullOrWhiteSpace(NewCategory) ? "ТО" : NewCategory.Trim(),
                Mileage = mileageAtSwap,
                PartCost = partCost,
                LaborCost = laborCost,
                ServiceName = string.IsNullOrWhiteSpace(NewServiceName) ? "Сам в гараже" : NewServiceName.Trim(),
                Notes = NewNotes?.Trim() ?? string.Empty,
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
        }

        EditingRecordId = null;
        NewPartTitle = string.Empty;
        NewPartCost = 0m;
        NewLaborCost = 0m;
        NewNotes = string.Empty;
        NewDate = DateTimeOffset.Now;
        RecordErrorMessage = string.Empty;
        IsAddingRecord = false;

        OnPropertyChanged(nameof(TotalGarageSpent));
        OnPropertyChanged(nameof(TotalRecordsCount));
    }

    // --- ЭКСПОРТ ДАННЫХ (Markdown, TXT, JSON) ---

    [RelayCommand]
    private void ExportSelectedVehicle(string? format)
    {
        if (SelectedVehicle == null)
        {
            ExportStatusMessage = "Для экспорта сначала добавьте или выберите автомобиль.";
            return;
        }

        try
        {
            string fmt = string.IsNullOrWhiteSpace(format) ? "md" : format;
            string path = ExportService.SaveReport(SelectedVehicle, fmt);
            ExportStatusMessage = $"✓ Отчёт сохранён: {System.IO.Path.GetFileName(path)}";
        }
        catch (Exception ex)
        {
            ExportStatusMessage = $"Ошибка экспорта: {ex.Message}";
        }
    }
}
