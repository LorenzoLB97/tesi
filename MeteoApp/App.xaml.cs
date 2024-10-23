using System.Collections.ObjectModel;
using System.Diagnostics;

namespace MeteoApp;

public partial class App : Application
{
    private static MyDatabase _database; // Variabile privata per memorizzare l'istanza

    public static MyDatabase Database
    {
        get
        {
            // Se il database non è ancora stato inizializzato, crealo
            if (_database == null)
                _database = new MyDatabase();

            // Restituisci l'istanza del database
            return _database;
        }
    }

    public App()
	{
        SQLitePCL.Batteries_V2.Init(); // Inizializza SQLite
        InitializeComponent();

		MainPage = new MeteoListPage();
	}

    // Metodo che viene eseguito quando l'app si avvia (solo avvio, non ripresa dopo standby)
    protected override void OnStart()
    {
        base.OnStart();

        loadDBEntries();
        
        var reference = MainPage as MeteoListPage;

        reference.GetCurrentLocation();
    }

    private async void loadDBEntries()
    {
        MeteoListPage reference = MainPage as MeteoListPage;
        Debug.WriteLine("AAAAAAAAAAAAAAA QUI1");

        if (_database != null)
        {
            Debug.WriteLine("AAAAAAAAAAAAAAA QUI2");

            ObservableCollection<Entry> loadedEntries = new ObservableCollection<Entry>(Database.GetEntries());

            if (loadedEntries.Count > 1) //esistono già delle personalEntries
            {
                (reference.BindingContext as MeteoListViewModel).Entries = loadedEntries;
            }
        }
    }
}