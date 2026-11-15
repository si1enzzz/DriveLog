# 🚗 AutoCare Tracker

> **Кроссплатформенное десктопное приложение для учёта обслуживания автомобилей и контроля расходов на содержание автопарка.**  
> Разработано на **C# 13** и **.NET 10** с использованием фреймворка **Avalonia UI** и архитектурного паттерна **MVVM**.

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-13.0-239120?logo=csharp&logoColor=white)](https://learn.microsoft.com/dotnet/csharp/)
[![Avalonia UI](https://img.shields.io/badge/Avalonia_UI-11.x-8A2BE2?logo=avalonia&logoColor=white)](https://avaloniaui.net/)
[![Architecture](https://img.shields.io/badge/Architecture-MVVM-00599C)](https://learn.microsoft.com/dotnet/architecture/maui/mvvm)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

---

## ✨ Ключевые возможности

* 🚘 **Управление несколькими автомобилями (Гараж):**
  * Добавление автомобилей с детальными характеристиками (марка, модель, поколение, год выпуска, объём двигателя, цвет, начальный пробег).
  * Индивидуальный счётчик расходов (`TotalSpent`) и истории обслуживания для каждой машины.
  * Безопасное удаление с валидацией (защита от случайного удаления единственного авто в гараже).

* 🔍 **Интеллектуальный справочник авто (База Avito / Auto.ru):**
  * Каталог из **59 автомобильных брендов** со всеми популярными моделями и их поколениями.
  * Чёткое разделение смежных моделей (например, **Kia K5** и **Kia Optima** вынесены в отдельные позиции со всеми своими поколениями).
  * **Каскадный автокомплит:** выбор марки мгновенно фильтрует список моделей, а выбор модели подгружает доступные поколения.
  * **Нормализация раскладки (Homoglyph Resolution):** умный поиск распознаёт русские буквы-двойники («к5», «х5», «а4») при вводе с русской клавиатуры.

* 📅 **Умная синхронизация дат и поколений:**
  * **Smart Generation-to-Year:** при выборе поколения (например, `IV (BD, 2018–2021)`) поле года выпуска автоматически выставляет начальный год поколения (`2018`).
  * **Smart Date Formatting:** для замен, выполненных в текущем календарном году, дата выводится в виде дня и месяца (`1 ноября`), а для прошлых лет автоматически добавляется год (`15 сентября 2024 г.`).

* 🎨 **Современный строгий Clean Dark UI:**
  * Палитра высокой контрастности (фон `#001114`, поверхности карточек `#07343D`, сигнальный зелёный `#0CA61E`, рубиновый `#E03E3E`).
  * **Modal Backdrop Dimming:** при открытии любой формы ввода фоновый интерфейс плавно затемняется, фокусируя внимание на задаче.
  * **Dismiss on Click-Outside:** закрытие модального окна кликом в затемнённую область фона или кнопкой «Отмена».
  * Никаких визуальных стикеров или дешёвых эмодзи — чистый инженерный стиль.

---

## 🏛 Архитектура и инженерные решения

Проект построен по строгому стандарту **MVVM (Model - View - ViewModel)**:

```mermaid
graph LR
    View[Views: MainWindow.axaml] <-->|Data Binding & Commands| ViewModel[ViewModels: MainViewModel.cs]
    ViewModel <-->|Business Logic & Collections| Model[Models: Vehicle & ServiceRecord]
    ViewModel -->|Query $O(1)$| Service[Services: CarCatalogService.cs]
```

* **Separation of Concerns:** интерфейс не содержит логики в Code-Behind (`MainWindow.axaml.cs` пустой). Все расчёты, валидация и фильтрация живут в `MainViewModel`.
* **Compiled Bindings:** строгая типизация привязок (`x:DataType="vm:MainViewModel"`), гарантирующая проверку свойств на этапе компиляции.
* **CommunityToolkit.Mvvm:** использование современных Source Generators (`[ObservableProperty]`, `[RelayCommand]`, `[NotifyPropertyChangedFor]`).
* **Defensive Programming:** сервис каталога защищён интерфейсом `IReadOnlyList<T>`, исключающим непреднамеренную модификацию базы извне.

> 📖 **Подробный разбор архитектурных компромиссов (ADR 001 – ADR 011) доступен в файле [engineering_decisions.md](engineering_decisions.md).**

---

## 📁 Структура проекта

```text
├── AutoCareApp/
│   ├── AutoCareApp/
│   │   ├── Models/               # Сущности предметной области (Vehicle, ServiceRecord)
│   │   ├── ViewModels/           # Реактивное состояние и команды (MainViewModel)
│   │   ├── Views/                # XAML-разметка окон и стили (MainWindow.axaml)
│   │   └── Services/             # Справочник авто и алгоритмы поиска (CarCatalogService)
│   └── AutoCareApp.slnx          # Решение .NET 10
├── engineering_decisions.md      # Architecture Decision Records (ADR)
├── csharp_guide.md               # Полный справочник по языку C#
├── avalonia_guide.md             # Руководство по Avalonia UI и XAML
└── README.md                     # Документация проекта
```

---

## 🚀 Запуск проекта

### Требования
* [.NET 10 SDK](https://dotnet.microsoft.com/download)
* Visual Studio 2022 / 2025 или JetBrains Rider / VS Code с поддержкой C#

### Сборка и запуск через CLI
```bash
# Клонировать репозиторий
git clone https://github.com/si1enzzz/app-to-drivers.git
cd app-to-drivers

# Запуск приложения
dotnet run --project AutoCareApp/AutoCareApp
```

---

## 👨‍💻 Автор
* **GitHub:** [@si1enzzz](https://github.com/si1enzzz)
* Проект подготовлен в рамках портфолио для поступления в профильные университеты (Software Engineering).
