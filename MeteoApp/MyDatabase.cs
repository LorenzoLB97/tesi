using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using SQLite;
using MeteoApp.Models;

namespace MeteoApp
{
    public static class Constants
    {
        public const string DatabaseFilename = "MyDatabase.db3";

        public const SQLite.SQLiteOpenFlags Flags =
            // open the database in read/write mode
            SQLite.SQLiteOpenFlags.ReadWrite |
            // create the database if it doesn't exist
            SQLite.SQLiteOpenFlags.Create |
            // enable multi-threaded database access
            SQLite.SQLiteOpenFlags.SharedCache;

        public static string DatabasePath => Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
    }
    /**
     * Il nostro database deve essere un singleton, in modo tale da poter essere
     * sempre accessibile globalmente
     * Creiamo il database dunque nella classe APP, dato che essa stessa é un singleton
     */
    public class MyDatabase
    {
        private SQLiteConnection _database {  get; set; }

        public MyDatabase()
        {
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MyDatabase.db");
            _database = new SQLiteConnection(dbPath);
            _database.CreateTable<Entry>();
            _database.CreateTable<CurrentLocationEntry>();           
        }

        public List<Entry> GetEntries()
        {
            return _database.Table<Entry>().ToList();
        }

        public CurrentLocationEntry GetCurrentLocationEntry()
        {
            return _database.Table<CurrentLocationEntry>().FirstOrDefault();
        }

        public int SaveEntry(Entry entry)
        {
            return _database.Insert(entry);
        }

        public void UpsertCurrentLocation(CurrentLocationEntry entry)
        {
            // Controlla se esiste già un record nella tabella
            var existingEntry = _database.Table<CurrentLocationEntry>().FirstOrDefault();

            if (existingEntry != null)
            {
                // Aggiorna il record esistente
                entry.Id = existingEntry.Id; // Mantieni lo stesso ID per garantire l'aggiornamento
                _database.Update(entry);
            }
            else
            {
                // Inserisci un nuovo record
                _database.Insert(entry);
            }
        }
    }
}
