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
    private readonly AppwriteService _appwriteService;

    public static bool isFirstRun;

    public App(IServiceProvider services)
	{
        SQLitePCL.Batteries_V2.Init(); // Inizializza SQLite
        InitializeComponent();

        Services = services;

        // Inietta le dipendenze tramite il provider di servizi
        _database = services.GetRequiredService<MyDatabase>();
        _appwriteService = services.GetRequiredService<AppwriteService>();
        _mainPage = services.GetRequiredService<MeteoListPage>();

        //Appwrite settings
        isFirstRun = Preferences.Get("isFirstRun", true);
        if (isFirstRun)
        {
            PerformFirstTimeSetup();
        }

        // Imposta la pagina principale come _mainPage
        MainPage = _mainPage;
    }

    // Metodo che viene eseguito quando l'app si avvia (solo avvio, non ripresa dopo standby)
    protected async override void OnStart()
    {
        base.OnStart();

        if (isFirstRun)
        {
            await LoadAppwriteEntriesOnLocalDB();
        }

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

    private async void PerformFirstTimeSetup()
    {
        await _appwriteService.CreateAppwriteDB();
    }

    //da testare FIXARE
    private async Task LoadAppwriteEntriesOnLocalDB()
    {
        try
        {
            // Recupera tutte le entries da Appwrite
            var entriesFromAppwrite = await _appwriteService.GetAllEntriesAsync();

            if (entriesFromAppwrite == null || !entriesFromAppwrite.Any())
            {
                Debug.WriteLine("Nessuna entry trovata su Appwrite.");
                return;
            }

            // Itera su ogni entry e aggiungila al database locale
            foreach (var entry in entriesFromAppwrite)
            {
                _database.SaveEntryFromAppwrite(entry);
                Debug.WriteLine($"Entry salvata nel database locale: {entry.CompleteAddress}");
            }

            Preferences.Set("isFirstRun", false); 

        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Errore durante la sincronizzazione delle entries: {ex.Message}");
        }
    }
}