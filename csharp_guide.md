# Полный справочник-конспект по C# (с нуля до создания приложений)

Этот конспект составлен на основе академической программы (уроки C# 1–10, ООП, LINQ, асинхронность) и современных стандартов C# (.NET 8/9). Он поможет быстро освежить память, понимать структуру любого кода и уверенно читать то, что пишет ИИ.

---

## Содержание
1. [Базовые типы данных и переменные](#1-базовые-типы-данных-и-переменные)
2. [Операторы и управляющие конструкции](#2-операторы-и-управляющие-конструкции)
3. [Массивы (Arrays)](#3-массивы-arrays)
4. [Строки (Strings) и форматирование](#4-строки-strings-и-форматирование)
5. [Перечисления (enum)](#5-перечисления-enum)
6. [ООП: Классы, объекты, свойства](#6-ооп-классы-объекты-свойства)
7. [ООП: Наследование и полиморфизм](#7-ооп-наследование-и-полиморфизм)
8. [ООП: Интерфейсы и абстрактные классы](#8-ооп-интерфейсы-и-абстрактные-классы)
9. [Обобщения (Generics) и Коллекции](#9-обобщения-generics-и-коллекции)
10. [Делегаты, события и лямбда-выражения](#10-делегаты-события-и-лямбда-выражения)
11. [LINQ (Запросы к данным в коде)](#11-linq-запросы-к-данным-в-коде)
12. [Обработка ошибок и исключений (Exceptions)](#12-обработка-ошибок-и-исключений-exceptions)
13. [Асинхронность: async / await и Task](#13-асинхронность-async--await-и-task)
14. [Работа с файлами и JSON](#14-работа-с-файлами-и-json)
15. [Шпаргалка: Быстрый синтаксический справочник](#15-шпаргалка-быстрый-синтаксический-справочник)

---

## 1. Базовые типы данных и переменные

C# — это **строго типизированный язык**. Каждая переменная имеет строго определённый тип.

### Основные типы:
| Тип в C# | Системный тип | Что хранит | Пример |
|---|---|---|---|
| `int` | `Int32` | Целое число (-2 млрд .. +2 млрд) | `int mileage = 145000;` |
| `long` | `Int64` | Большое целое число | `long bigNum = 3000000000L;` |
| `double` | `Double` | Число с плавающей точкой (8 байт) | `double dailyRate = 35.5;` |
| `decimal` | `Decimal` | Точные дробные числа для **денег** (16 байт) | `decimal partCost = 4500.50m;` |
| `bool` | `Boolean` | Логическое значение (`true` / `false`) | `bool isOverdue = false;` |
| `string` | `String` | Текст (в двойных кавычках) | `string title = "Замена масла";` |
| `char` | `Char` | Один символ (в одинарных кавычках) | `char grade = 'A';` |
| `DateTime` | `DateTime` | Дата и время | `DateTime today = DateTime.Now;` |

### Неявная типизация: `var`
Компилятор сам определяет тип переменной по значению справа:
```csharp
var mileage = 120000;       // компилятор поймет, что это int
var carName = "Lada Vesta"; // string
var isServiced = true;      // bool
// Тип фиксируется навсегда! carName = 5; вызовет ошибку!
```

### Nullable-типы (значения, которые могут быть null)
Обычные числа (`int`, `double`) не могут быть `null`. Чтобы разрешить им быть пустыми, ставится знак `?`:
```csharp
int? lastServiceMileage = null; // пробега может ещё не быть!

if (lastServiceMileage.HasValue)
{
    Console.WriteLine($"Пробег: {lastServiceMileage.Value}");
}
// Или через оператор объединения с null (??):
int displayMileage = lastServiceMileage ?? 0; // если null, возьми 0
```

---

## 2. Операторы и управляющие конструкции

### Условия: `if / else if / else`
```csharp
int wearPercent = 92;

if (wearPercent >= 100)
{
    Console.WriteLine("Срочно заменить деталь!");
}
else if (wearPercent >= 85)
{
    Console.WriteLine("Скоро замена (внимание).");
}
else
{
    Console.WriteLine("Состояние узла в норме.");
}
```

### Тернарный оператор `? :` (короткий if)
```csharp
string status = wearPercent >= 100 ? "Просрочено" : "В норме";
```

### Современный `switch` (Switch Expression)
В современном C# switch пишется невероятно компактно:
```csharp
string category = "Oil";

string intervalText = category switch
{
    "Oil" => "Каждые 8 000 км",
    "SparkPlugs" => "Каждые 30 000 км",
    "Brakes" => "Каждые 40 000 км",
    _ => "Регламент не задан" // _ это default
};
```

### Циклы: `for`, `foreach`, `while`
```csharp
// 1. Классический for (по счетчику)
for (int i = 0; i < 5; i++)
{
    Console.WriteLine($"Запись #{i + 1}");
}

// 2. foreach — самый частый цикл (перебор любой коллекции/списка)
string[] parts = { "Масло", "Свечи", "Колодки" };
foreach (var part in parts)
{
    Console.WriteLine($"Расходник: {part}");
}

// 3. while (пока условие истинно)
int currentKm = 0;
while (currentKm < 1000)
{
    currentKm += 250;
}
```

---

## 3. Массивы (Arrays)

Массив — это упорядоченный набор элементов фиксированной длины.

```csharp
// Создание и инициализация
int[] mileages = new int[3]; // массив на 3 элемента [0, 0, 0]
mileages[0] = 120000;
mileages[1] = 125000;
mileages[2] = 132000;

// Быстрая инициализация
string[] services = { "Гараж", "СТО Вираж", "Официальный дилер" };

// Полезные методы из класса System.Array:
Array.Sort(mileages);                  // Сортировка по возрастанию
Array.Reverse(mileages);               // Развернуть массив задом наперед
int count = mileages.Length;           // Количество элементов (длина)
bool exists = mileages.Contains(120000); // Метод расширения LINQ
```

> ⚠️ **Внимание:** Размер массива нельзя изменить после создания! Если нужен список, в который можно свободно добавлять и удалять элементы — используют `List<T>` (см. раздел 9).

---

## 4. Строки (Strings) и форматирование

В C# строки (`string`) — **неизменяемые (immutable)**. Любая операция со строкой создаёт новую строку.

### Интерполяция строк `$""` (самый удобный способ)
```csharp
string car = "Kia Rio";
int mileage = 87500;
decimal cost = 3200.50m;

// Вставка переменных прямо в строку через {}
string message = $"Авто: {car}, текущий пробег: {mileage:N0} км, стоимость: {cost:C2}";
// Вывод: Авто: Kia Rio, текущий пробег: 87 500 км, стоимость: 3 200,50 ₽
```

### Популярные методы строк:
```csharp
string rawInput = "  Масло моторное 5W-40  ";

string clean = rawInput.Trim();               // "Масло моторное 5W-40" (убирает пробелы по краям)
string lower = clean.ToLower();               // "масло моторное 5w-40"
bool hasOil = lower.Contains("масло");         // true (содержит ли подстроку)
bool startsWith = clean.StartsWith("Масло");   // true

// Замена и разбиение
string replaced = clean.Replace("5W-40", "5W-30");
string[] words = clean.Split(' ');            // разбиение на массив слов

// Проверка на пустоту (очень важно для форм ввода!):
if (string.IsNullOrWhiteSpace(rawInput))
{
    Console.WriteLine("Пользователь ничего не ввел!");
}
```

### `StringBuilder`
Если в цикле нужно объединить тысячи строк, обычный `+` тратит много памяти. Используют `StringBuilder`:
```csharp
using System.Text;

var sb = new StringBuilder();
for (int i = 0; i < 100; i++)
{
    sb.AppendLine($"Лог калибровки #{i}");
}
string fullReport = sb.ToString();
```

---

## 5. Перечисления (enum)

`enum` позволяет объявить список именованных констант вместо того, чтобы писать строки вроде `"В норме"`, `"Скоро"`, `"Просрочено"` (где легко сделать опечатку).

```csharp
// Объявление
public enum MaintenanceStatus
{
    Normal,     // 0
    Warning,    // 1 (скоро замена)
    Overdue     // 2 (просрочено)
}

public enum PartCategory
{
    Engine,
    Brakes,
    Suspension,
    Transmission,
    Other
}

// Использование:
MaintenanceStatus status = MaintenanceStatus.Warning;

if (status == MaintenanceStatus.Overdue)
{
    Console.WriteLine("Срочно в сервис!");
}
```

---

## 6. ООП: Классы, объекты, свойства

Класс — это чертёж (шаблон), а объект — это конкретный экземпляр, созданный по этому чертежу.

```csharp
public class Vehicle
{
    // 1. Поля (внутренние переменные, обычно private)
    private double _dailyRate;

    // 2. Свойства (Properties) — публичный доступ к данным
    public int Id { get; set; }                      // Автоматическое свойство
    public string Brand { get; set; } = string.Empty; // Значение по умолчанию
    public string Model { get; set; } = string.Empty;
    public int CurrentMileage { get; set; }

    // Свойство только для чтения (вычисляемое на лету):
    public string FullName => $"{Brand} {Model}";

    // Свойство с проверкой логики (полная запись):
    public double AvgDailyMileage
    {
        get => _dailyRate;
        set
        {
            // Защита от дурака: пробег не может быть отрицательным или космическим
            if (value < 0) _dailyRate = 0;
            else if (value > 500) _dailyRate = 500;
            else _dailyRate = value;
        }
    }

    // 3. Конструктор (вызывается при создании через new)
    public Vehicle(string brand, string model, int currentMileage)
    {
        Brand = brand;
        Model = model;
        CurrentMileage = currentMileage;
        AvgDailyMileage = 30.0; // безопасный дефолт (30 км/день)
    }

    // Пустой конструктор (часто нужен для баз данных SQLite / EF Core)
    public Vehicle() { }

    // 4. Методы (действие, которое объект умеет делать)
    public void UpdateMileage(int newMileage, int daysPassed)
    {
        if (newMileage > CurrentMileage && daysPassed > 0)
        {
            int deltaKm = newMileage - CurrentMileage;
            AvgDailyMileage = (double)deltaKm / daysPassed;
            CurrentMileage = newMileage;
        }
    }
}
```

### Создание и использование объекта:
```csharp
// Создание объекта
Vehicle myCar = new Vehicle("Kia", "Rio", 90000);

// Чтение и вызов методов:
Console.WriteLine(myCar.FullName); // "Kia Rio"
myCar.UpdateMileage(90600, 20);    // проехал 600 км за 20 дней -> темп 30 км/день
Console.WriteLine($"Новый пробег: {myCar.CurrentMileage}, темп: {myCar.AvgDailyMileage} км/д");
```

---

## 7. ООП: Наследование и полиморфизм

**Наследование** позволяет создать новый класс на основе существующего, переняв его свойства и методы.

```csharp
// Базовый класс (родитель)
public class BaseRecord
{
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    // virtual означает, что дочерний класс МОЖЕТ изменить поведение метода
    public virtual void PrintInfo()
    {
        Console.WriteLine($"Запись #{Id} создана {CreatedDate:d}");
    }
}

// Дочерний класс (наследник через двоеточие :)
public class ServiceRecord : BaseRecord
{
    public string PartTitle { get; set; } = string.Empty;
    public decimal Cost { get; set; }

    // override переопределяет метод базового класса
    public override void PrintInfo()
    {
        base.PrintInfo(); // вызов метода родителя
        Console.WriteLine($"  Деталь: {PartTitle}, Цена: {Cost:C}");
    }
}
```

### Полиморфизм:
Мы можем работать с разными типами через общий родительский тип:
```csharp
BaseRecord record = new ServiceRecord { Id = 1, PartTitle = "Свечи", Cost = 2400m };
record.PrintInfo(); // Вызовется именно версия из ServiceRecord!
```

---

## 8. ООП: Интерфейсы и абстрактные классы

Интерфейс — это **строгий контракт**: он говорит *ЧТО* должен уметь делать класс, но не говорит *КАК*.

Именование интерфейсов в C# всегда начинается с буквы `I`:
```csharp
// Описание контракта для фоновых уведомлений:
public interface INotificationScheduler
{
    void Schedule(string title, string message, DateTime notifyTime);
    void Cancel(int id);
}

// Реализация для Android:
public class AndroidNotificationService : INotificationScheduler
{
    public void Schedule(string title, string message, DateTime notifyTime)
    {
        // Код под Android AlarmManager
        Console.WriteLine($"[Android] Запланировано на {notifyTime}: {message}");
    }

    public void Cancel(int id)
    {
        Console.WriteLine($"[Android] Отмена уведомления #{id}");
    }
}
```

> 💡 **Зачем это нужно для мобильного приложения:** Avalonia не знает специфику Android или iOS. Но через интерфейс `INotificationScheduler` наш общий интерфейс UI может запланировать напоминание, а платформа Android сама выполнит свою реализацию!

---

## 9. Обобщения (Generics) и Коллекции

Вместо массивов в реальной разработке всегда используют коллекции из пространства имен `System.Collections.Generic`.

### 1. `List<T>` — динамический список (самая частая коллекция!)
```csharp
List<string> parts = new List<string>();

// Добавление
parts.Add("Моторное масло");
parts.Add("Масляный фильтр");
parts.Add("Воздушный фильтр");

// Удаление
parts.Remove("Воздушный фильтр");

// Доступ по индексу
string first = parts[0];

// Количество
int total = parts.Count;

// Проверка наличия
bool hasOil = parts.Contains("Моторное масло");
```

### 2. `Dictionary<TKey, TValue>` — словарь (пары ключ-значение)
Позволяет моментально находить значение по уникальному ключу (без перебора всего списка):
```csharp
// Словарь: [Категория] -> [Рекомендуемый интервал в км]
var intervals = new Dictionary<string, int>
{
    ["Oil"] = 8000,
    ["Plugs"] = 30000,
    ["BrakeFluid"] = 40000
};

// Чтение по ключу:
int oilInterval = intervals["Oil"]; // 8000

// Безопасное чтение (если ключа может не быть):
if (intervals.TryGetValue("Transmission", out int transInterval))
{
    Console.WriteLine($"Интервал коробки: {transInterval}");
}
else
{
    Console.WriteLine("Категория не найдена");
}
```

---

## 10. Делегаты, события и лямбда-выражения

### Что такое Лямбда-выражение?
Лямбда `=>` — это короткая анонимная функция. Читается как: *"переменная слева переходит в выражение справа"*.
```csharp
// Стрелочная функция: x переходит в x * 2
Func<int, int> doubleNum = x => x * 2;
int result = doubleNum(5); // 10
```

### Встроенные делегаты `Action` и `Func`:
* `Action` — функция, которая **ничего не возвращает** (`void`).
* `Func<T, TResult>` — функция, которая принимает аргументы и **возвращает результат**.

```csharp
Action<string> log = msg => Console.WriteLine($"[LOG]: {msg}");
log("Приложение запущено"); // вывод на экран

Func<int, int, int> add = (a, b) => a + b;
int sum = add(10, 20); // 30
```

---

## 11. LINQ (Запросы к данным в коде)

**LINQ (Language Integrated Query)** — одна из лучших возможностей C#. Это возможность фильтровать, сортировать и преобразовывать списки данных так же просто, как писать SQL.

Пусть у нас есть список деталей:
```csharp
public class PartItem
{
    public string Title { get; set; } = "";
    public int WearPercent { get; set; }
    public decimal Cost { get; set; }
    public bool IsCritical { get; set; }
}

var parts = new List<PartItem>
{
    new() { Title = "Масло 5W-40", WearPercent = 95, Cost = 4500m, IsCritical = true },
    new() { Title = "Колодки", WearPercent = 40, Cost = 2500m, IsCritical = true },
    new() { Title = "Свечи", WearPercent = 88, Cost = 1800m, IsCritical = false },
    new() { Title = "Лампочка фары", WearPercent = 10, Cost = 300m, IsCritical = false }
};
```

### Главные методы LINQ:

```csharp
using System.Linq;

// 1. Where — фильтрация (найти детали с износом >= 85%)
var needService = parts.Where(p => p.WearPercent >= 85).ToList();

// 2. OrderBy / OrderByDescending — сортировка
var sortedByWear = parts.OrderByDescending(p => p.WearPercent).ToList();

// 3. Select — трансформация (получить только названия деталей)
List<string> titles = parts.Select(p => p.Title).ToList();

// 4. FirstOrDefault — найти первый элемент или null
PartItem? mostWorn = parts.OrderByDescending(p => p.WearPercent).FirstOrDefault();

// 5. Any / All — проверки
bool hasCriticalOverdue = parts.Any(p => p.IsCritical && p.WearPercent >= 100); // true/false
bool allSafe = parts.All(p => p.WearPercent < 80);

// 6. Агрегатные функции: Sum, Average, Count, Max, Min
decimal totalCost = parts.Sum(p => p.Cost);       // сумма всех цен
double avgWear = parts.Average(p => p.WearPercent); // средний износ
int urgentCount = parts.Count(p => p.WearPercent >= 85);
```

---

## 12. Обработка ошибок и исключений (Exceptions)

Когда в программе что-то идёт не так (нет файла, нет сети, деление на ноль, неверный ввод), выбрасывается **Exception**. Если его не поймать — приложение упадёт.

```csharp
try
{
    // Опасный код: парсинг ввода пользователя
    string userInput = "abc";
    int mileage = int.Parse(userInput); // вызовет FormatException
}
catch (FormatException ex)
{
    // Сюда попадаем при ошибке формата числа
    Console.WriteLine($"Ошибка ввода: введите только цифры! ({ex.Message})");
}
catch (Exception ex)
{
    // Перехват любой другой непредвиденной ошибки
    Console.WriteLine($"Неизвестная ошибка: {ex.Message}");
}
finally
{
    // Выполняется ВСЕГДА (даже если была ошибка) — закрытие файлов, соединений
    Console.WriteLine("Завершение операции.");
}
```

### Вызов своего исключения: `throw`
```csharp
public void SetMileage(int newKm, int oldKm)
{
    if (newKm < oldKm)
    {
        throw new ArgumentException("Новый пробег не может быть меньше текущего!");
    }
}
```

---

## 13. Асинхронность: async / await и Task

> ⚡ **Зачем это в телефоне:** Если приложение выполняет долгую операцию (чтение базы данных SQLite, запрос к серверу, расчет графиков) в главном потоке — **экран телефона зависает**, кнопки не нажимаются, операционная система считает приложение зависшим.
> Ключевые слова `async` и `await` позволяют выполнять долгие задачи в фоне, не блокируя пользовательский интерфейс!

```csharp
// Метод с пометкой async возвращает Task или Task<T>
public async Task<List<ServiceRecord>> LoadRecordsFromDbAsync()
{
    Console.WriteLine("Начало чтения из базы в фоне...");
    
    // Имитация долгого чтения (или реальный вызов context.Records.ToListAsync())
    await Task.Delay(1500); 

    return new List<ServiceRecord>
    {
        new() { PartTitle = "Тормозные диски", Cost = 7000m }
    };
}

// Вызов асинхронного метода:
public async Task OnRefreshButtonClicked()
{
    IsLoading = true; // показать спиннер загрузки на экране
    
    // await "ждет" завершения в фоне, при этом UI остается живым и отзывчивым!
    List<ServiceRecord> records = await LoadRecordsFromDbAsync();
    
    IsLoading = false; // скрыть спиннер
}
```

---

## 14. Работа с файлами и JSON

Сохранить настройки, журнал или список авто в файл можно через `System.Text.Json`:

```csharp
using System.IO;
using System.Text.Json;

public class AppSettings
{
    public string DefaultCarName { get; set; } = "Моё Авто";
    public double DefaultDailyRate { get; set; } = 35.0;
}

// 1. Сохранение объекта в JSON-файл:
var settings = new AppSettings { DefaultCarName = "Lada Vesta", DefaultDailyRate = 42.0 };
string jsonString = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
File.WriteAllText("settings.json", jsonString);

// 2. Чтение объекта из JSON-файла:
if (File.Exists("settings.json"))
{
    string readJson = File.ReadAllText("settings.json");
    AppSettings? loaded = JsonSerializer.Deserialize<AppSettings>(readJson);
    Console.WriteLine($"Загружено: {loaded?.DefaultCarName}");
}
```

---

## 15. Шпаргалка: Быстрый синтаксический справочник

| Конструкция | Синтаксис | Назначение |
|---|---|---|
| Переменная | `int km = 1000;` / `var km = 1000;` | Создать переменную |
| Nullable | `int? x = null;` | Число, которое может быть пустым |
| Null-коалесценция | `int val = x ?? 0;` | Если x null, взять 0 |
| Интерполяция строк | `$"Пробег: {km} км"` | Вставка переменных в строку |
| Условие | `if (a > b) { } else { }` | Ветвление |
| Тернарный оператор | `int max = a > b ? a : b;` | Короткий if-else |
| Switch-выражение | `val switch { 1 => "A", _ => "B" }` | Компактный выбор |
| Список | `List<int> list = new(); list.Add(5);` | Динамический массив |
| LINQ фильтр | `list.Where(x => x > 10).ToList()` | Отфильтровать список |
| LINQ поиск | `list.FirstOrDefault(x => x == 5)` | Найти первый подходящий |
| Класс и свойство | `public class A { public int Id { get; set; } }` | Описание модели данных |
| Лямбда-функция | `x => x * 2` | Анонимная функция |
| Асинхронный метод | `public async Task<int> GetDataAsync()` | Фоновый метод |
| Вызов асинхронного | `int result = await GetDataAsync();` | Ожидание без зависания UI |
