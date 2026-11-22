using System;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using AutoCareApp.Models;

namespace AutoCareApp.Services;

/// <summary>
/// Сервис для экспорта данных об автомобиле и истории его обслуживания в форматы Markdown, TXT и JSON.
/// </summary>
public static class ExportService
{
    /// <summary>
    /// Экспорт в формат Markdown (.md) с красивой таблицей и структурированными блоками.
    /// </summary>
    public static string ToMarkdown(Vehicle vehicle)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"# 🚗 {vehicle.Title}");
        sb.AppendLine();
        sb.AppendLine($"- **Характеристики:** {vehicle.SubDetails}");
        sb.AppendLine($"- **Текущий пробег:** {vehicle.CurrentMileage:N0} км");
        sb.AppendLine($"- **Всего инвестировано в обслуживание:** {vehicle.TotalSpent:N0} ₽");
        sb.AppendLine($"- **Дата выгрузки отчёта:** {DateTime.Now:dd.MM.yyyy HH:mm}");
        sb.AppendLine();

        sb.AppendLine($"## 📋 Журнал замен и обслуживания ({vehicle.Records.Count})");
        sb.AppendLine();

        if (vehicle.Records.Count == 0)
        {
            sb.AppendLine("_Записей об обслуживании пока нет._");
            return sb.ToString();
        }

        sb.AppendLine("| Дата | Пробег | Что меняли | Сервис | Запчасть | Работа | Итого | Заметка |");
        sb.AppendLine("| :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- |");

        foreach (var r in vehicle.Records)
        {
            string note = string.IsNullOrWhiteSpace(r.Notes) ? "—" : r.Notes.Replace("|", "/");
            sb.AppendLine($"| {r.Date:dd.MM.yyyy} | {r.Mileage:N0} км | {r.PartTitle} | {r.ServiceName} | {r.PartCost:N0} ₽ | {r.LaborCost:N0} ₽ | **{r.TotalCost:N0} ₽** | {note} |");
        }

        return sb.ToString();
    }

    /// <summary>
    /// Экспорт в понятный текстовый формат (.txt), удобный для печати или отправки в мессенджерах.
    /// </summary>
    public static string ToPlainText(Vehicle vehicle)
    {
        var sb = new StringBuilder();
        sb.AppendLine("==================================================");
        sb.AppendLine($" ЖУРНАЛ ОБСЛУЖИВАНИЯ: {vehicle.Title}");
        sb.AppendLine("==================================================");
        sb.AppendLine($"Характеристики: {vehicle.SubDetails}");
        sb.AppendLine($"Текущий пробег: {vehicle.CurrentMileage:N0} км");
        sb.AppendLine($"Всего расходов: {vehicle.TotalSpent:N0} ₽");
        sb.AppendLine($"Дата выгрузки:  {DateTime.Now:dd.MM.yyyy HH:mm}");
        sb.AppendLine("--------------------------------------------------");
        sb.AppendLine($"ИСТОРИЯ ОБСЛУЖИВАНИЯ ({vehicle.Records.Count} записей):");
        sb.AppendLine("--------------------------------------------------");

        if (vehicle.Records.Count == 0)
        {
            sb.AppendLine("Записей пока нет.");
        }
        else
        {
            int index = 1;
            foreach (var r in vehicle.Records)
            {
                sb.AppendLine($"[{index++}] {r.Date:dd.MM.yyyy} (пробег {r.Mileage:N0} км)");
                sb.AppendLine($"    Деталь/работа: {r.PartTitle}");
                sb.AppendLine($"    Сервис:        {r.ServiceName}");
                sb.AppendLine($"    Запчасть:      {r.PartCost:N0} ₽ | Работа: {r.LaborCost:N0} ₽ | Итого: {r.TotalCost:N0} ₽");
                if (!string.IsNullOrWhiteSpace(r.Notes))
                {
                    sb.AppendLine($"    Заметка:       {r.Notes}");
                }
                sb.AppendLine();
            }
        }

        sb.AppendLine("==================================================");
        return sb.ToString();
    }

    /// <summary>
    /// Экспорт в машиночитаемый структурированный JSON (.json) с поддержкой кириллицы.
    /// </summary>
    public static string ToJson(Vehicle vehicle)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        var exportDto = new
        {
            ExportDate = DateTime.Now,
            Vehicle = new
            {
                vehicle.Id,
                vehicle.Brand,
                vehicle.Model,
                vehicle.Generation,
                vehicle.Year,
                vehicle.EngineVolume,
                vehicle.Color,
                vehicle.CurrentMileage,
                vehicle.TotalSpent,
                RecordsCount = vehicle.Records.Count,
                Records = vehicle.Records
            }
        };

        return JsonSerializer.Serialize(exportDto, options);
    }

    /// <summary>
    /// Сохранение отчёта в файл на диске с автогенерацией имени.
    /// </summary>
    public static string SaveReport(Vehicle vehicle, string extension, string? targetDirectory = null)
    {
        string directory = targetDirectory ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "DriveLogReports"
        );

        Directory.CreateDirectory(directory);

        string ext = extension.TrimStart('.').ToLowerInvariant();
        string safeBrand = vehicle.Brand.Replace(" ", "_");
        string safeModel = vehicle.Model.Replace(" ", "_");
        string fileName = $"{safeBrand}_{safeModel}_{DateTime.Now:yyyyMMdd_HHmm}.{ext}";
        string fullPath = Path.Combine(directory, fileName);

        string content = ext switch
        {
            "md" => ToMarkdown(vehicle),
            "txt" => ToPlainText(vehicle),
            "json" => ToJson(vehicle),
            _ => throw mechanicalException(ext)
        };

        File.WriteAllText(fullPath, content, Encoding.UTF8);
        return fullPath;

        static ArgumentException mechanicalException(string format) =>
            new($"Неподдерживаемый формат экспорта: {format}. Доступны: md, txt, json.");
    }
}
