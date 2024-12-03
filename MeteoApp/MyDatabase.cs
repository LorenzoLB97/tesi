using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using SQLite;
using MeteoApp.Services;
using System.Diagnostics;

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
        private SQLiteConnection Database {  get; set; }
        private int CurrentLocationEntryId { get; set; } = 1;

        private readonly AppwriteService _appwriteService;

        public MyDatabase(AppwriteService appwriteService)
        {
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MyDatabase.db");
            Database = new SQLiteConnection(dbPath);
            Database.CreateTable<Entry>();

            _appwriteService = appwriteService;
        }

        public List<Entry> GetEntries()
        {
            return Database.Table<Entry>().ToList();
        }

        /**
         * ritorna la current location, che é sempre il primo elemento della lista.
         */
        public Entry GetCurrentLocationEntry()
        {
            return Database.Table<Entry>().ToList().FirstOrDefault();
        }

        //Soluzione: fare 2 saveEntry, una solo per la sincronizzazione con appwrite? (SI, é la piu semplice soluzione)
        public async Task<int> SaveEntry(Entry entry)
        {
            Debug.WriteLine("XXXXX DENTRO A SAVEENTRY, FIRSTRUN? " + App.isFirstRun);
            //await _appwriteService.appWriteTestConnection();
            
            await _appwriteService.SaveEntryAsync(entry);
            
            return Database.Insert(entry);
        }

        public int SaveEntryFromAppwrite(Entry entry)
        {
            return Database.Insert(entry);
        }

        /**
         * Questo metodo deve sostituire la currentLocation
         * Se la tabella é vuota, semplicemente aggiunge la nuova entry
         * Se la tabella non é vuota, deve trovare la entry isCurrentLocation=true (che é la CurrentLocation)
         * e sostuiturla con la nuova currentLocation.
         */
        public void UpsertCurrentLocation(Entry newCurrentLocationEntry)
        {
            // Find the existing current location entry
            var existingEntry = Database.Table<Entry>().FirstOrDefault(e => e.IsCurrentLocation);

            if (existingEntry != null)
            {
                // Ensure the new entry has the same Id as the existing one
                newCurrentLocationEntry.Id = existingEntry.Id;
                // Update the existing entry
                Database.Update(newCurrentLocationEntry);
            }
            else
            {
                // Insert the new current location entry
                Database.Insert(newCurrentLocationEntry);
            }
        }


        public async void Remove(Entry entryToRemove)
        {
            await _appwriteService.DeleteEntryAsync(entryToRemove);
            Database.Delete(entryToRemove);
        }

        public int GetCurrentLocationId()
        {
            return CurrentLocationEntryId;
        }
    }
}
