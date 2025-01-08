using System.Diagnostics;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Microsoft.Maui.Controls;
using MeteoApp.Services;

namespace MeteoApp
{
    public partial class MapPage : ContentPage
    {
        private readonly GeoLocationService _geoLocationService;
        private readonly MyDatabase _database;

        // Pin temporaneo che mostriamo alla posizione cliccata
        private Pin _tempPin;

        public MapPage(GeoLocationService geoLocationService, MyDatabase database)
        {
            InitializeComponent();

            _geoLocationService = geoLocationService;
            _database = database;

            // Imposta la posizione iniziale della mappa su currentLocation
            Entry currentLocation = _database.GetCurrentLocationEntry();
            map.MoveToRegion(
                MapSpan.FromCenterAndRadius(
                    new Location(currentLocation.Latitude, currentLocation.Longitude),
                    Distance.FromMiles(5)
                )
            );
        }

        private async void OnSearchButtonPressed(object sender, EventArgs e)
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
                    map.MoveToRegion(
                        MapSpan.FromCenterAndRadius(
                            new Location(location.Latitude, location.Longitude),
                            Distance.FromMiles(1)
                        )
                    );
                }
                else
                {
                    await DisplayAlert("Errore", "Indirizzo non trovato.", "OK");
                }
            }
        }

        private async void OnMapClicked(object sender, MapClickedEventArgs e)
        {
            Debug.WriteLine("OnMapClicked: utente ha cliccato sulla mappa.");

            // Rimuoviamo eventuale pin esistente prima di crearne uno nuovo
            if (_tempPin != null)
            {
                map.Pins.Remove(_tempPin);
                _tempPin = null;
            }

            // Crea un nuovo pin rosso alla posizione cliccata
            _tempPin = new Pin
            {
                Label = "Nuova posizione",
                Location = e.Location,
                Type = PinType.Place // Su Android, di default è rosso
            };
            map.Pins.Add(_tempPin);

            // Esegui il Reverse Geocoding per ottenere i dati dell'indirizzo
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

                // In ogni caso (OK o Cancel), rimuoviamo il pin
                map.Pins.Remove(_tempPin);
                _tempPin = null;
            }
            else
            {
                await DisplayAlert("Errore", "Non è stato possibile ottenere l'indirizzo per questa posizione.", "OK");

                // Rimuoviamo il pin anche in caso di errore
                map.Pins.Remove(_tempPin);
                _tempPin = null;
            }
        }
    }
}
