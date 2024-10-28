using Microsoft.Maui.Controls.Maps;
using Map = Microsoft.Maui.Controls.Maps.Map;

namespace MeteoApp;

public partial class MapPage : ContentPage
{    
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

    void OnMapClicked(object sender, MapClickedEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("AAAAAAAAAAAAAAAAAAAA" + $"MapClick: {e.Location.Latitude}, {e.Location.Longitude}");

        //Reverse GeoCoding
        

        // Mostra l'alert con le coordinate
        DisplayAlert("Location Selected", $"Latitude: {e.Location.Latitude}\nLongitude: {e.Location.Longitude}", "OK");
    }
}