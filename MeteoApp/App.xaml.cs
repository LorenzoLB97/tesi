using MeteoApp.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace MeteoApp;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; }

    private readonly MyDatabase _database; // Variabile privata per memorizzare l'istanza
    private readonly MeteoListPage _mainPage;
    private readonly GeoLocationService _geoLocationService;

    //private Timer _timer;

    public App(IServiceProvider services)
	{
        SQLitePCL.Batteries_V2.Init(); // Inizializza SQLite
        InitializeComponent();

        // Imposta il timer per eseguire la funzione ogni 5 minuti
        /*_timer = new Timer(async (e) =>
        {
            await ExecuteBackgroundLocationTask();
        }, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));*/

        Services = services;

        // Inietta le dipendenze tramite il provider di servizi
        _database = services.GetRequiredService<MyDatabase>();
        _mainPage = services.GetRequiredService<MeteoListPage>();
        //_geoLocationService = services.GetRequiredService<GeoLocationService>();

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