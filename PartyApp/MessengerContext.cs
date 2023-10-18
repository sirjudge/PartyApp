using System;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace PartyApp;

/// <summary>
/// DbContext class modeled after the example from the Microsoft docs:
/// https://learn.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli
/// </summary>
public class MessengerContext : DbContext
{
    public string DbPath {get;}
    
    public MessengerContext(string databaseName)
    {
        databaseName = databaseName += ".db";
        // if file exists dont delete and return early
        if (File.Exists(databaseName)) return;

        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = Path.Join(path, databaseName);
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}