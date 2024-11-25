using Microsoft.AspNetCore.Components.WebView.Maui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Map = Microsoft.Maui.Controls.Maps.Map;

namespace MeteoApp
{
    public partial class WeatherBlazorPage : ContentPage
    {
        public static WeatherInfo SharedWeatherInfo { get; private set; }

        public WeatherBlazorPage(WeatherInfo weatherInfo)
        {
            InitializeComponent();
            SharedWeatherInfo = weatherInfo; // Rende l'oggetto disponibile alla pagina Blazor
        }
    }
}

