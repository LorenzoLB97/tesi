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

        /**al posto di usare una referenza statica possiamo usare WeakReferenceMessenger
         * ma chiediamo prima a Galli dato che é da installare da NuGet
         * Migliore: cerchiamo di usare un database
         */
        var reference = MainPage as MeteoListPage;

        reference.GetCurrentLocation();
    }
}