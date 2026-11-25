using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DriveLog.Models;
using SQLite;

namespace DriveLog.Services;

public class DatabaseService
{
    private static DatabaseService? _instance;
    public static DatabaseService Instance => _instance ??= new DatabaseService();

    private readonly SQLiteConnection? _db;

    public DatabaseService(string? customPath = null)
    {
        try
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
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[DatabaseService] Init error: {ex}");
        }
    }

    private static string GetDefaultDbPath()
    {
        string basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        if (string.IsNullOrEmpty(basePath))
        {
            basePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        }

#if ANDROID
        return Path.Combine(basePath, "drivelog.db");
#else
        string dir = Path.Combine(basePath, "DriveLog");
        if (!Directory.Exists(dir))
        {
            try { Directory.CreateDirectory(dir); } catch { }
        }
        return Path.Combine(dir, "drivelog.db");
#endif
    }

    // --- ОПЕРАЦИИ С АВТОМОБИЛЯМИ ---

    public List<Vehicle> GetAllVehicles()
    {
        if (_db == null) return new List<Vehicle>();
        try
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
        catch
        {
            return new List<Vehicle>();
        }
    }

    public int SaveVehicle(Vehicle vehicle)
    {
        if (_db == null) return vehicle.Id;
        try
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
        catch
        {
            return vehicle.Id;
        }
    }

    public void DeleteVehicle(int vehicleId)
    {
        if (_db == null) return;
        try
        {
            _db.Table<ServiceRecord>().Delete(r => r.VehicleId == vehicleId);
            _db.Delete<Vehicle>(vehicleId);
        }
        catch { }
    }

    // --- ОПЕРАЦИИ С ЗАПИСЯМИ ТО ---

    public List<ServiceRecord> GetRecordsForVehicle(int vehicleId)
    {
        if (_db == null) return new List<ServiceRecord>();
        try
        {
            return _db.Table<ServiceRecord>()
                      .Where(r => r.VehicleId == vehicleId)
                      .OrderByDescending(r => r.Date)
                      .ThenByDescending(r => r.Id)
                      .ToList();
        }
        catch
        {
            return new List<ServiceRecord>();
        }
    }

    public int SaveRecord(ServiceRecord record)
    {
        if (_db == null) return record.Id;
        try
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
        catch
        {
            return record.Id;
        }
    }

    public void DeleteRecord(int recordId)
    {
        if (_db == null) return;
        try
        {
            _db.Delete<ServiceRecord>(recordId);
        }
        catch { }
    }
}
