using MeteoApp.Services;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Maps;
using Map = Microsoft.Maui.Controls.Maps.Map;

namespace MeteoApp;

public partial class MapPage : ContentPage
{
    private readonly GeoLocationService _geoLocationService;

    public MapPage()
    {
        InitializeComponent();

        Map map = new Map()
        {
            IsShowingUser = true
        };
        map.MapClicked += OnMapClicked;
        Content = map;
    }

    async void OnMapClicked(object sender, MapClickedEventArgs e)
    {
        //Questo codice non é una ripetizione ridondante: purtroppo il metodo geocoding getplacemarkAsync ci mette molto tempo
        //e riscrivere il codice di reverse geo coding qui velocizza il processo
        System.Diagnostics.Debug.WriteLine("AAAAAAAAAAAAAAAAAAAA" + $"MapClick: {e.Location.Latitude}, {e.Location.Longitude}");

        var placemarks = await Geocoding.GetPlacemarksAsync(e.Location.Latitude, e.Location.Longitude); 
        var placemark = placemarks?.FirstOrDefault();

        System.Diagnostics.Debug.WriteLine(" FINALLY XXXXXXXXXXXXXXXXXXX" + $"MapClick: {e.Location.Latitude}, {e.Location.Longitude}");

        if (placemark != null)
        {
            string completeAddress = $"{placemark.Thoroughfare}, {placemark.Locality}, {placemark.PostalCode}, {placemark.CountryName}";
            string street = placemark.Thoroughfare;
            string city = placemark.Locality;
            string postalCode = placemark.PostalCode;
            string country = placemark.CountryName;

            // Mostra l'alert con l'indirizzo completo
            await DisplayAlert("Indirizzo Selezionato", $"{completeAddress}", "OK");

            App.Database.SaveEntry(new Entry()
            {
                CompleteAddress = completeAddress,
                Street = street,
                City = city,
                PostalCode = postalCode,
                Country = country
            });
        }
        else
        {
            // Mostra un messaggio di errore se l'indirizzo non è disponibile
            await DisplayAlert("Errore", "Non è stato possibile determinare l'indirizzo.", "OK");
        }
    }
}