using MeteoApp.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace MeteoApp;

public partial class MapPage : ContentPage
{
    private readonly GeoLocationService _geoLocationService;
    private readonly MyDatabase _database;

    public MapPage(GeoLocationService geoLocationService, MyDatabase database)
    {
        InitializeComponent();

        _geoLocationService = geoLocationService;
        _database = database;

        // Imposta la posizione iniziale della mappa su currentLocation
        Entry currentLocation = _database.GetCurrentLocationEntry();
        map.MoveToRegion(MapSpan.FromCenterAndRadius(new Location(currentLocation.Latitude, currentLocation.Longitude), Distance.FromMiles(5)));
    }

    async void OnSearchButtonPressed(object sender, EventArgs e)
    {
        string address = searchBar.Text;

        if (!string.IsNullOrEmpty(address))
        {
            // Usa il servizio di geocoding per ottenere le coordinate
            var locations = await Geocoding.GetLocationsAsync(address);
            var location = locations?.FirstOrDefault();

            if (location != null)
            {
                // Sposta la mappa sulla posizione trovata
                map.MoveToRegion(MapSpan.FromCenterAndRadius(new Location(location.Latitude, location.Longitude), Distance.FromMiles(1)));
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
