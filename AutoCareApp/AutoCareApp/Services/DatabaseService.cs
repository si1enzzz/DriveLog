using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoCareApp.Models;
using SQLite;

namespace AutoCareApp.Services;

public class DatabaseService
{
    private static DatabaseService? _instance;
    public static DatabaseService Instance => _instance ??= new DatabaseService();

    private readonly SQLiteConnection _db;

    public DatabaseService(string? customPath = null)
    {
        string dbPath = customPath ?? GetDefaultDbPath();
        var directory = Path.GetDirectoryName(dbPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        _db = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
        _db.CreateTable<Vehicle>();
        _db.CreateTable<ServiceRecord>();
    }

    private static string GetDefaultDbPath()
    {
        string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return Path.Combine(appData, "AutoCareApp", "autocare.db");
    }

    // --- ОПЕРАЦИИ С АВТОМОБИЛЯМИ ---

    public List<Vehicle> GetAllVehicles()
    {
        var vehicles = _db.Table<Vehicle>().ToList();
        foreach (var v in vehicles)
        {
            var records = GetRecordsForVehicle(v.Id);
            v.Records.Clear();
            foreach (var r in records)
            {
                v.Records.Add(r);
            }
        }
        return vehicles;
    }

    public int SaveVehicle(Vehicle vehicle)
    {
        if (vehicle.Id == 0)
        {
            _db.Insert(vehicle);
        }
        else
        {
            _db.Update(vehicle);
        }
        return vehicle.Id;
    }

    public void DeleteVehicle(int vehicleId)
    {
        // Каскадное удаление: сначала удаляем все записи ТО этой машины
        _db.Table<ServiceRecord>().Delete(r => r.VehicleId == vehicleId);
        // Затем удаляем сам автомобиль
        _db.Delete<Vehicle>(vehicleId);
    }

    // --- ОПЕРАЦИИ С ЗАПИСЯМИ ТО ---

    public List<ServiceRecord> GetRecordsForVehicle(int vehicleId)
    {
        return _db.Table<ServiceRecord>()
                  .Where(r => r.VehicleId == vehicleId)
                  .OrderByDescending(r => r.Date)
                  .ThenByDescending(r => r.Id)
                  .ToList();
    }

    public int SaveRecord(ServiceRecord record)
    {
        if (record.Id == 0)
        {
            _db.Insert(record);
        }
        else
        {
            _db.Update(record);
        }
        return record.Id;
    }

    public void DeleteRecord(int recordId)
    {
        _db.Delete<ServiceRecord>(recordId);
    }
}
