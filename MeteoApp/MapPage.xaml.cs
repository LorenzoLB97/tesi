using MeteoApp.Services;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Map = Microsoft.Maui.Controls.Maps.Map;

namespace MeteoApp;

public partial class MapPage : ContentPage
{
    private readonly GeoLocationService _geoLocationService;
    private readonly MyDatabase _database;
    private Map _map; // Rendi la mappa accessibile da altri metodi
    private SearchBar _searchBar; // Barra di ricerca

    public MapPage(GeoLocationService geoLocationService, MyDatabase database)
    {
        InitializeComponent();

        _geoLocationService = geoLocationService;
        _database = database;

        // Creazione della barra di ricerca
        _searchBar = new SearchBar
        {
            Placeholder = "Cerca una località"
        };
        _searchBar.SearchButtonPressed += OnSearchButtonPressed;

        // Creazione della mappa e impostazione della posizione iniziale sulla currentLocation
        Entry currentLocation = _database.GetCurrentLocationEntry();
        _map = new Map(MapSpan.FromCenterAndRadius(new Location(currentLocation.Latitude, currentLocation.Longitude), Distance.FromMiles(5)))
        {
            IsShowingUser = true
        };
        _map.MapClicked += OnMapClicked;

        // Aggiungi la barra di ricerca e la mappa allo StackLayout
        Content = new StackLayout
        {
            Children =
            {
                _searchBar,
                _map
            }
        };
    }

    async void OnSearchButtonPressed(object sender, EventArgs e)
    {
        string address = _searchBar.Text;

        if (!string.IsNullOrEmpty(address))
        {
            // Usa il servizio di geocoding per ottenere le coordinate
            var locations = await Geocoding.GetLocationsAsync(address);
            var location = locations?.FirstOrDefault();

            if (location != null)
            {
                // Sposta la mappa sulla posizione trovata
                _map.MoveToRegion(MapSpan.FromCenterAndRadius(new Location(location.Latitude, location.Longitude), Distance.FromMiles(1)));
            }
            else
            {
                await DisplayAlert("Errore", "Indirizzo non trovato.", "OK");
            }
        }
    }

    async void OnMapClicked(object sender, MapClickedEventArgs e)
    {
        Entry newEntry = await _geoLocationService.ReverseGeoCoding(e.Location);
        if (newEntry != null)
        {
            bool isConfirmed = await DisplayAlert(
                "Aggiungi Location",
                $"Vuoi aggiungere la seguente location?\n\n{newEntry.CompleteAddress}",
                "OK",
                "Cancel"
            );

            if (isConfirmed)
            {
                _database.SaveEntry(newEntry);
                await DisplayAlert("Successo", "Location aggiunta con successo!", "OK");
            }
        }
        else
        {
            await DisplayAlert("Errore", "Non è stato possibile ottenere l'indirizzo per questa posizione.", "OK");
        }
    }
}
