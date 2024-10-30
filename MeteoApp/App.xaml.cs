using System.Collections.ObjectModel;
using System.Diagnostics;

namespace MeteoApp;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; }

    private static MyDatabase _database; // Variabile privata per memorizzare l'istanza
    private readonly MeteoListPage _mainPage;


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

        // Inietta le dipendenze tramite il provider di servizi
        _database = services.GetRequiredService<MyDatabase>();
        _mainPage = services.GetRequiredService<MeteoListPage>();

        // Imposta la pagina principale come _mainPage
        MainPage = _mainPage;
    }

    // Metodo che viene eseguito quando l'app si avvia (solo avvio, non ripresa dopo standby)
    protected override void OnStart()
    {
        base.OnStart();

        // Carica le entries del database
        LoadDBEntries();

        // Chiama GetCurrentLocation nella pagina principale
        _mainPage.GetCurrentLocation();
    }

    private void LoadDBEntries()
    {
        var reference = _mainPage as MeteoListPage;

        ObservableCollection<Entry> loadedEntries = new ObservableCollection<Entry>(_database.GetEntries());

        if (loadedEntries.Count > 1) //esistono già delle personalEntries
        {
            (reference.BindingContext as MeteoListViewModel).Entries = loadedEntries;
        }
    }
}