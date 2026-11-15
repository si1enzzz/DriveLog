# Полный справочник-конспект: Avalonia UI + MVVM + SQLite для мобильных и десктоп приложений

Этот конспект обобщает материалы курсов по Avalonia UI (уроки 1–9, верстка, MVVM, EF Core) и адаптирует их под разработку современного мобильного и десктопного приложения на C#.

---

## Содержание
1. [Что такое Avalonia UI и почему она](#1-что-такое-avalonia-ui-и-почему-она)
2. [Структура проекта Avalonia](#2-структура-проекта-avalonia)
3. [Основы разметки XAML (AXAML)](#3-основы-разметки-xaml-axaml)
4. [Контейнеры компоновки (Layout)](#4-контейнеры-компоновки-layout)
5. [Базовые элементы управления (Controls)](#5-базовые-элементы-управления-controls)
6. [Стили, ресурсы и карточный дизайн](#6-стили-ресурсы-и-карточный-дизайн)
7. [Привязка данных (Data Binding)](#7-привязка-данных-data-binding)
8. [Архитектура MVVM (Model - View - ViewModel)](#8-архитектура-mvvm-model---view---viewmodel)
9. [CommunityToolkit.Mvvm: современный MVVM без рутины](#9-communitytoolkitmvvm-современный-mvvm-без-рутины)
10. [Списки и коллекции в UI: ItemsControl и ListBox](#10-списки-и-коллекции-в-ui-itemscontrol-и-listbox)
11. [База данных SQLite + EF Core в Avalonia](#11-база-данных-sqlite--ef-core-в-avalonia)
12. [Мобильные нюансы и системные напоминания](#12-мобильные-нюансы-и-системные-напоминания)
13. [Полный рабочий пример экрана трекера авто](#13-полный-рабочий-пример-экрана-трекера-авто)
14. [Шпаргалка по XAML и командам](#14-шпаргалка-по-xaml-и-командам)

---

## 1. Что такое Avalonia UI и почему она

**Avalonia UI** — это современный кроссплатформенный фреймворк для .NET. В отличие от других решений, Avalonia рисует каждый пиксель интерфейса сама через графический движок **Skia** (тот же движок, что используется в Google Chrome и Flutter).

### Преимущества:
- **Один и тот же UI код** работает на: Android, iOS, Windows, macOS, Linux и в браузере (WebAssembly).
- **Синтаксис XAML** — мощный декларативный язык разметки интерфейса (как HTML, но строго типизированный).
- **MVVM из коробки** — четкое разделение внешнего вида и логики расчетов.

---

## 2. Структура проекта Avalonia

В решении под мобильные устройства (Cross-Platform) проект обычно делится на части:

```
CarTrackerApp.sln
│
├── CarTrackerApp (Общая библиотека — 95% всего кода!)
│   ├── Models/               (Классы данных: Vehicle, ServiceRecord, PartItem)
│   ├── ViewModels/           (Логика экранов: MainViewModel, AddRecordViewModel)
│   ├── Views/                (Экраны XAML: MainView.axaml)
│   ├── Services/             (Калькулятор пробега, База SQLite, Напоминания)
│   └── App.axaml / App.axaml.cs (Точка входа, общие стили и темы)
│
├── CarTrackerApp.Desktop     (Точка запуска для Windows/macOS/Linux)
└── CarTrackerApp.Android     (Точка запуска для смартфона Android + MainActivity)
```

---

## 3. Основы разметки XAML (AXAML)

Интерфейс в Avalonia описывается в файлах с расширением `.axaml` с помощью XML-тегов:
Каждый тег — это класс C#, а каждый атрибут — это свойство класса!

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:CarTrackerApp.ViewModels"
             x:Class="CarTrackerApp.Views.MainView"
             x:DataType="vm:MainViewModel">

    <!-- Внутри располагаются визуальные элементы -->
    <TextBlock Text="Привет, автолюбитель!" 
               FontSize="20" 
               Foreground="#2B2D42" />
</UserControl>
```

---

## 4. Контейнеры компоновки (Layout)

Элементы на экране не ставятся по жестким координатам (X, Y), иначе на разных телефонах экран "поедет". Вместо этого используются контейнеры:

### 1. `Grid` (Сетка) — самый мощный контейнер
Делит пространство на строки (`RowDefinitions`) и столбцы (`ColumnDefinitions`).
Размеры задаются тремя способами:
- `Auto` — размер ровно по содержимому.
- `*` (звёздочка) — занять всю оставшуюся пропорциональную часть экрана.
- Число (например `50`) — фиксированный размер в пикселях.

```xml
<Grid RowDefinitions="Auto, *, Auto" ColumnDefinitions="*, 120">
    <!-- Строка 0, Колонка 0 -->
    <TextBlock Grid.Row="0" Grid.Column="0" Text="Текущий пробег:" FontWeight="Bold" />
    
    <!-- Строка 0, Колонка 1 -->
    <TextBlock Grid.Row="0" Grid.Column="1" Text="142 500 км" Foreground="Green" />

    <!-- Строка 1, растянута на обе колонки (ColumnSpan="2") -->
    <ListBox Grid.Row="1" Grid.ColumnSpan="2" />

    <!-- Строка 2 (подвал) -->
    <Button Grid.Row="2" Grid.ColumnSpan="2" Content="Обновить пробег" HorizontalAlignment="Stretch" />
</Grid>
```

### 2. `StackPanel` (Стек)
Размещает элементы последовательно один за другим: сверху вниз (`Orientation="Vertical"`, по умолчанию) или слева направо (`Orientation="Horizontal"`).

```xml
<StackPanel Orientation="Vertical" Spacing="10" Margin="16">
    <TextBlock Text="Калибровка одометра" FontSize="18" FontWeight="SemiBold" />
    <TextBox Watermark="Введите точный пробег с приборной панели" />
    <Button Content="Сохранить" HorizontalAlignment="Right" />
</StackPanel>
```

### 3. `ScrollViewer` (Прокрутка)
На смартфонах контент часто не помещается на одном экране. Оберните его в `ScrollViewer`:
```xml
<ScrollViewer>
    <StackPanel Spacing="15">
        <!-- Много карточек и полей ввода -->
    </StackPanel>
</ScrollViewer>
```

---

## 5. Базовые элементы управления (Controls)

| Элемент | Для чего | Пример XAML |
|---|---|---|
| `TextBlock` | Вывод текста | `<TextBlock Text="Масло 5W-40" FontSize="16"/>` |
| `TextBox` | Ввод текста | `<TextBox Text="{Binding PartName}" Watermark="Название детали"/>` |
| `Button` | Кнопка | `<Button Content="Добавить запись" Command="{Binding AddRecordCommand}"/>` |
| `CheckBox` | Галочка | `<CheckBox Content="Замена выполнена на СТО" IsChecked="{Binding AtService}"/>` |
| `NumericUpDown` | Ввод чисел (цена/км) | `<NumericUpDown Value="{Binding Mileage}" Minimum="0" FormatString="N0"/>` |
| `ProgressBar` | Шкала износа детали | `<ProgressBar Value="{Binding WearPercent}" Maximum="100" Height="8"/>` |
| `Border` | Карточка/рамка со скруглением | `<Border CornerRadius="12" Background="White" Padding="15">` |

---

## 6. Стили, ресурсы и карточный дизайн

Современный мобильный интерфейс строится на **карточках** со скругленными краями и мягкими тенями.

```xml
<!-- Красивая карточка автомобиля -->
<Border Background="#1E1E24" 
        CornerRadius="16" 
        Padding="20" 
        Margin="12"
        BoxShadow="0 4 15 0 #30000000">
    <StackPanel Spacing="8">
        <TextBlock Text="Kia Rio IV (2018)" Foreground="White" FontSize="20" FontWeight="Bold"/>
        
        <Grid ColumnDefinitions="*, Auto">
            <TextBlock Text="Расчетный пробег сегодня:" Foreground="#A0AAB2" VerticalAlignment="Center"/>
            <TextBlock Grid.Column="1" Text="124 850 км" Foreground="#4EBA6F" FontSize="22" FontWeight="Bold"/>
        </Grid>
        
        <TextBlock Text="Темп: ~34 км/день (калибровка 5 дней назад)" Foreground="#707A82" FontSize="12"/>
    </StackPanel>
</Border>
```

---

## 7. Привязка данных (Data Binding)

Главный принцип Avalonia: **мы никогда не пишем в C# код вроде `myTextBox.Text = "123"`**. 
Вместо этого мы привязываем свойство визуального элемента к свойству в нашей C# модели:

```xml
<!-- Односторонняя привязка: UI читает значение из C# переменной -->
<TextBlock Text="{Binding CarMileageText}" />

<!-- Двусторонняя привязка: пользователь вводит текст -> он тут же пишется в C# свойство -->
<TextBox Text="{Binding InputMileage, Mode=TwoWay}" />

<!-- Привязка клика кнопки к C# методу (Команде) -->
<Button Content="Калибровать" Command="{Binding CalibrateCommand}" />
```

---

## 8. Архитектура MVVM (Model - View - ViewModel)

MVVM делит приложение на 3 независимых слоя:
```
┌─────────────────┐        Data Binding         ┌─────────────────────┐
│      VIEW       │ ◄─────────────────────────► │      VIEWMODEL      │
│  (XAML экран)   │          (Команды)          │   (Логика экрана)   │
└─────────────────┘                             └──────────┬──────────┘
                                                           │ Работает с
                                                           ▼
                                                ┌─────────────────────┐
                                                │        MODEL        │
                                                │ (Сущности БД / Данные)
                                                └─────────────────────┘
```

1. **Model (Модель):** Чистые данные (классы `Vehicle`, `ServiceRecord`).
2. **View (Представление):** Файл `.axaml` (разметка кнопок, цветов, текста).
3. **ViewModel (Модель представления):** C# класс, где живут свойства для экрана и команды нажатия кнопок.

---

## 9. CommunityToolkit.Mvvm: современный MVVM без рутины

Раньше для обновления экрана при изменении переменной приходилось вручную писать десятки строк с событием `PropertyChanged`.
Библиотека **CommunityToolkit.Mvvm** делает это автоматически через специальные атрибуты:

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

// Наследуемся от ObservableObject
public partial class MainViewModel : ObservableObject
{
    // [ObservableProperty] автоматически создаст публичное свойство 'CurrentMileage'
    // и уведомит экран Avalonia при его изменении!
    [ObservableProperty]
    private int _currentMileage = 120000;

    [ObservableProperty]
    private string _carName = "Kia Rio";

    [ObservableProperty]
    private double _dailyRate = 32.5;

    // [RelayCommand] автоматически создаст команду 'UpdateOdometerCommand' для кнопки!
    [RelayCommand]
    private void UpdateOdometer()
    {
        CurrentMileage += 500; // Экран сам мгновенно обновит цифру!
    }

    [RelayCommand]
    private async Task SaveDataAsync()
    {
        // Асинхронное сохранение в SQLite
        await Task.Delay(500);
    }
}
```

---

## 10. Списки и коллекции в UI: ItemsControl и ListBox

Чтобы отобразить список расходников автомобиля, обычный `List<T>` не подходит (он не умеет уведомлять экран о добавлении элементов).
Используется **`ObservableCollection<T>`**:

```csharp
using System.Collections.ObjectModel;

public partial class MainViewModel : ObservableObject
{
    // Коллекция для экрана
    public ObservableCollection<PartCardViewModel> Parts { get; } = new();

    public void LoadParts()
    {
        Parts.Clear();
        Parts.Add(new PartCardViewModel { Title = "Масло 5W-40", WearPercent = 92, StatusColor = "#E63946" });
        Parts.Add(new PartCardViewModel { Title = "Тормозные колодки", WearPercent = 45, StatusColor = "#2A9D8F" });
    }
}
```

### Отображение списка карточек в XAML:
```xml
<ItemsControl ItemsSource="{Binding Parts}">
    <ItemsControl.ItemTemplate>
        <DataTemplate>
            <!-- Шаблон одной карточки -->
            <Border Background="#252836" CornerRadius="12" Margin="0, 0, 0, 10" Padding="14">
                <Grid ColumnDefinitions="*, Auto" RowDefinitions="Auto, Auto">
                    <TextBlock Grid.Row="0" Grid.Column="0" Text="{Binding Title}" Foreground="White" FontWeight="Bold"/>
                    <TextBlock Grid.Row="0" Grid.Column="1" Text="{Binding WearPercent, StringFormat='{}{0}%'}" Foreground="{Binding StatusColor}"/>
                    
                    <!-- Шкала износа -->
                    <ProgressBar Grid.Row="1" Grid.ColumnSpan="2" Margin="0,8,0,0"
                                 Value="{Binding WearPercent}" Maximum="100" 
                                 Foreground="{Binding StatusColor}" Height="6"/>
                </Grid>
            </Border>
        </DataTemplate>
    </ItemsControl.ItemTemplate>
</ItemsControl>
```

---

## 11. База данных SQLite + EF Core в Avalonia

Для мобильного приложения идеально подходит локальная база данных **SQLite** в связке с **Entity Framework Core**.

### Шаг 1. Модель данных:
```csharp
public class ServiceRecord
{
    public int Id { get; set; }
    public string PartTitle { get; set; } = string.Empty;
    public int MileageAtSwap { get; set; }
    public decimal Cost { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
    public string ServiceName { get; set; } = string.Empty;
}
```

### Шаг 2. Контекст БД (`AppDbContext`):
```csharp
using Microsoft.EntityFrameworkCore;
using System.IO;

public class AppDbContext : DbContext
{
    public DbSet<ServiceRecord> Records => Set<ServiceRecord>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // Путь к файлу БД в папке приложения на телефоне/компьютере
        string dbPath = Path.Combine(System.Environment.GetFolderPath(
            System.Environment.SpecialFolder.LocalApplicationData), "autocare.db");

        options.UseSqlite($"Data Source={dbPath}");
    }
}
```

### Шаг 3. Инициализация при старте (в `App.axaml.cs`):
```csharp
using var db = new AppDbContext();
db.Database.EnsureCreated(); // Создаст файл autocare.db и таблицы, если их нет
```

---

## 12. Мобильные нюансы и системные напоминания

### Адаптация экрана под смартфон:
1. **Ширина и растяжение:** Не задавайте жесткий `Width="400"`. Используйте `HorizontalAlignment="Stretch"`.
2. **Размеры кнопок для пальца:** Минимальная высота кликабельного элемента на тач-экране — **44–48 пикселей** (`MinHeight="48"`).
3. **Безопасная зона (Safe Area):** Отступ сверху под "чёлку" и камеру телефона (обычно `Margin="16, 24, 16, 16"`).

### Локальные push-уведомления (паттерн Сервис):
Создаем интерфейс в общем коде:
```csharp
public interface ILocalNotificationService
{
    void ScheduleMaintenanceAlert(string title, string body, DateTime notifyAt);
}
```
В Android-проекте он реализуется через системный `AlarmManager` или `WorkManager`, а ViewModel просто вызывает:
```csharp
_notificationService.ScheduleMaintenanceAlert(
    "Замена масла!", 
    "Расчетный пробег достиг 128 000 км. Пора на ТО.", 
    targetDate);
```

---

## 13. Полный рабочий пример экрана трекера авто

Соберем всё вместе: реальный экран, который можно запускать и тестировать.

### ViewModel (`MainViewModel.cs`):
```csharp
using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CarTrackerApp.ViewModels;

public partial class PartItemViewModel : ObservableObject
{
    public string Title { get; set; } = "";
    public int WearPercent { get; set; }
    public string StatusText => WearPercent >= 100 ? "Просрочено!" : WearPercent >= 85 ? "Скоро замена" : "В норме";
    public string StatusColor => WearPercent >= 100 ? "#E63946" : WearPercent >= 85 ? "#F4A261" : "#2A9D8F";
}

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private string _carModel = "Kia Rio 1.6";

    [ObservableProperty]
    private int _estimatedMileage = 134250;

    [ObservableProperty]
    private double _dailyRate = 34.0;

    [ObservableProperty]
    private int _inputCalibrationMileage;

    public ObservableCollection<PartItemViewModel> Parts { get; } = new();

    public MainViewModel()
    {
        InputCalibrationMileage = EstimatedMileage;
        LoadDemoParts();
    }

    private void LoadDemoParts()
    {
        Parts.Add(new PartItemViewModel { Title = "Моторное масло 5W-40", WearPercent = 92 });
        Parts.Add(new PartItemViewModel { Title = "Свечи зажигания", WearPercent = 35 });
        Parts.Add(new PartItemViewModel { Title = "Тормозная жидкость", WearPercent = 104 });
    }

    [RelayCommand]
    private void CalibrateOdometer()
    {
        if (InputCalibrationMileage >= EstimatedMileage - 5000)
        {
            EstimatedMileage = InputCalibrationMileage;
            // Пересчитываем проценты износа узлов...
        }
    }
}
```

### View (`MainView.axaml`):
```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:vm="using:CarTrackerApp.ViewModels"
             x:Class="CarTrackerApp.Views.MainView"
             x:DataType="vm:MainViewModel"
             Background="#12131A">

    <ScrollViewer Padding="16">
        <StackPanel Spacing="16" MaxWidth="500">
            
            <!-- Карточка Автомобиля и Одометра -->
            <Border Background="#1C1E2A" CornerRadius="16" Padding="18">
                <StackPanel Spacing="10">
                    <TextBlock Text="{Binding CarModel}" FontSize="22" FontWeight="Bold" Foreground="White"/>
                    
                    <Grid ColumnDefinitions="*, Auto">
                        <StackPanel>
                            <TextBlock Text="Расчетный пробег сегодня" Foreground="#8E92A8" FontSize="13"/>
                            <TextBlock Text="{Binding EstimatedMileage, StringFormat='{}{0:N0} км'}" 
                                       FontSize="28" FontWeight="Black" Foreground="#4EBA6F"/>
                        </StackPanel>
                        
                        <StackPanel Grid.Column="1" VerticalAlignment="Center">
                            <TextBlock Text="Темп езды" Foreground="#8E92A8" FontSize="12" HorizontalAlignment="Right"/>
                            <TextBlock Text="{Binding DailyRate, StringFormat='{}~{0:F0} км/день'}" 
                                       Foreground="#D0D4E4" FontWeight="SemiBold" HorizontalAlignment="Right"/>
                        </StackPanel>
                    </Grid>

                    <!-- Поле быстрой калибровки -->
                    <Grid ColumnDefinitions="*, Auto" Margin="0, 8, 0, 0">
                        <NumericUpDown Value="{Binding InputCalibrationMileage}" 
                                       Minimum="0" Maximum="1000000" FormatString="N0"
                                       Height="42" Margin="0,0,8,0"/>
                        <Button Grid.Column="1" Content="Откалибровать" 
                                Command="{Binding CalibrateOdometerCommand}"
                                Height="42" Background="#3A86FF" Foreground="White" CornerRadius="8"/>
                    </Grid>
                </StackPanel>
            </Border>

            <!-- Заголовок списка расходников -->
            <TextBlock Text="Состояние узлов и расходников" FontSize="18" FontWeight="Bold" Foreground="White"/>

            <!-- Список деталей -->
            <ItemsControl ItemsSource="{Binding Parts}">
                <ItemsControl.ItemTemplate>
                    <DataTemplate>
                        <Border Background="#1C1E2A" CornerRadius="14" Margin="0,0,0,10" Padding="14">
                            <StackPanel Spacing="8">
                                <Grid ColumnDefinitions="*, Auto">
                                    <TextBlock Text="{Binding Title}" Foreground="White" FontWeight="SemiBold" FontSize="16"/>
                                    <TextBlock Grid.Column="1" Text="{Binding StatusText}" Foreground="{Binding StatusColor}" FontWeight="Bold"/>
                                </Grid>

                                <ProgressBar Value="{Binding WearPercent}" Maximum="100" Height="8" 
                                             Foreground="{Binding StatusColor}" Background="#2B2E40" CornerRadius="4"/>
                            </StackPanel>
                        </Border>
                    </DataTemplate>
                </ItemsControl.ItemTemplate>
            </ItemsControl>

        </StackPanel>
    </ScrollViewer>
</UserControl>
```

---

## 14. Шпаргалка по XAML и командам

| Задача в Avalonia | Решение в XAML / C# |
|---|---|
| Сделать адаптивную разметку | `<Grid RowDefinitions="Auto, *" ColumnDefinitions="*, 2*">` |
| Скруглить углы блока | `<Border CornerRadius="12" Background="#1E1E24">` |
| Показать список объектов | `<ItemsControl ItemsSource="{Binding Items}">` |
| Привязать свойство в две стороны | `Text="{Binding UserName, Mode=TwoWay}"` |
| Сделать команду нажатия кнопки | `[RelayCommand] private void DoAction() { }` |
| Создать реактивное поле ViewModel | `[ObservableProperty] private int _mileage;` |
| Список с реактивным обновлением | `public ObservableCollection<T> Items { get; } = new();` |
| Прокрутить длинный экран | `<ScrollViewer><StackPanel>...</StackPanel></ScrollViewer>` |
