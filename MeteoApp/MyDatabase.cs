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
        private SQLiteConnection Database {  get; set; }
        private int CurrentLocationEntryId { get; set; } = 1;

        public MyDatabase()
        {
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MyDatabase.db");
            Database = new SQLiteConnection(dbPath);
            Database.CreateTable<Entry>();
            //_database.CreateTable<CurrentLocationEntry>();           
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

        public int SaveEntry(Entry entry)
        {
            return Database.Insert(entry);
        }

        /**
         * Questo metodo deve sostituire la currentLocation
         * Se la tabella é vuota, semplicemente aggiunge la nuova entry
         * Se la tabella non é vuota, deve trovare la entry con id=1 (che é la CurrentLocation)
         * e sostuiturla con la nuova currentLocation.
         */
        public void UpsertCurrentLocation(Entry newCurrentLocationEntry)
        {
            // Trova la current location esistente
            var existingEntry = Database.Find<Entry>(CurrentLocationEntryId);

            if (existingEntry != null) // Se la current location esiste già
            {
                // Aggiorna l'entry esistente
                Database.Update(newCurrentLocationEntry);
            }
            else
            {                
                // Inserisci un nuovo record
                Database.Insert(newCurrentLocationEntry);
            }
        }

        public void Remove(Entry entryToRemove)
        {
            Database.Delete(entryToRemove);
        }

        public int GetCurrentLocationId()
        {
            return CurrentLocationEntryId;
        }
    }
}
