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
    public MapPage(GeoLocationService geoLocationService, MyDatabase database)
    {
        InitializeComponent();

        _geoLocationService = geoLocationService;
        _database = database;

        // Creazione della mappa e impostazione della posizione iniziale su (42, 42)
        Entry currentLocation = _database.GetCurrentLocationEntry();
        Map map = new Map(MapSpan.FromCenterAndRadius(new Location(currentLocation.Latitude, currentLocation.Longitude), Distance.FromMiles(5)))
        {
            IsShowingUser = true
        };
        map.MapClicked += OnMapClicked;
        Content = map;
    }

    async void OnMapClicked(object sender, MapClickedEventArgs e)
    {
        Entry newEntry = await _geoLocationService.ReverseGeoCoding(e.Location);
        //Richiesta all'utente di conferma della location
        if (newEntry != null)
        {
            // Mostra un messaggio di conferma
            bool isConfirmed = await DisplayAlert(
                "Aggiungi Location",
                $"Vuoi aggiungere la seguente location?\n\n{newEntry.CompleteAddress}",
                "OK",
                "Cancel"
            );

            // Se l'utente conferma, aggiunge l'entry al database
            if (isConfirmed)
            {
                // Chiama la funzione per salvare la location, sostituendo GeoLocationService con la logica corretta
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
