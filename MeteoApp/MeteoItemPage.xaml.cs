using MeteoApp.Services;
using System.Diagnostics;

namespace MeteoApp;

[QueryProperty(nameof(Entry), "Entry")]
public partial class MeteoItemPage : ContentPage
{
    private Entry entry;
    private WeatherInfo currentWeatherInfo;

    private readonly WeatherService _weatherService;

    public Entry Entry
    {
        get => entry;
        set
        {
            entry = value;
            OnPropertyChanged();
        }
    }

    public WeatherInfo WeatherInfo
    {
        get => currentWeatherInfo;
        set
        {
            currentWeatherInfo = value;
            OnPropertyChanged();
        }
    }

    public MeteoItemPage(WeatherService weatherService)
    {
        InitializeComponent();
        BindingContext = this;

        _weatherService = weatherService;
    }

    /**
     * Metodo di ContentPage
     * viene eseguito ogni volta che una pagina in un'applicazione 
     * .NET MAUI diventa visibile all'utente
     * Questo è utile in molti scenari, come:
     * Aggiornare i dati della pagina prima che venga mostrata.
     * Avviare processi, come recuperare dati da un'API o avviare animazioni.
     * Registrare eventi o gestire l'interfaccia utente che dipende dalla visibilità della pagina.
     */
    protected async override void OnAppearing()
    {
        base.OnAppearing();

        WeatherInfo = await _weatherService.GetCurrentWeatherAsync(entry);

        if (currentWeatherInfo != null)
        {
            Debug.WriteLine("XXXXXXXXXX Info: \n" +
            currentWeatherInfo.Base);

            Debug.WriteLine("XXXXXXXXXX Info Temp: \n" +
            "Temp: " + currentWeatherInfo.Main.Temp);
        } else
        {
            Debug.WriteLine("ERRORE: NULL?");
        }     
    }
}