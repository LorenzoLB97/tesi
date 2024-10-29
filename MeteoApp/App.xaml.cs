using System.Collections.ObjectModel;
using System.Diagnostics;

namespace MeteoApp;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; }

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

    public App(IServiceProvider services)
	{
        SQLitePCL.Batteries_V2.Init(); // Inizializza SQLite
        InitializeComponent();
        Services = services;

        // Usa il provider di servizi per creare l'istanza di MeteoListPage
        MainPage = services.GetRequiredService<MeteoListPage>();
    }

    // Metodo che viene eseguito quando l'app si avvia (solo avvio, non ripresa dopo standby)
    protected override void OnStart()
    {
        base.OnStart();

        LoadDBEntries();
        
        var reference = MainPage as MeteoListPage;

        reference.GetCurrentLocation();
    }

    private void LoadDBEntries()
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