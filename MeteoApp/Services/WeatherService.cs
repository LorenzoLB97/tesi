using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Diagnostics;
using static System.Net.WebRequestMethods;
using File = System.IO.File;
using Microsoft.Extensions.Configuration;

namespace MeteoApp.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public WeatherService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();

            // Ottieni l'API key dalla configurazione
            _apiKey = configuration["WeatherApiKey"];

            Console.WriteLine($"XXXXXXXXXXX API Key: {_apiKey}");
        }

        // Metodo per ottenere il meteo attuale per una determinata città
        public async Task<WeatherInfo> GetCurrentWeatherAsync(Entry entry)
        {

            double lat = entry.Latitude;
            double lon = entry.Longitude;

            string url = $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid={_apiKey}";

            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode(); // Lancia un'eccezione se lo status code non è 2xx

                var json = await response.Content.ReadAsStringAsync();
                WeatherInfo weatherInfo = new WeatherInfo();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                weatherInfo = JsonSerializer.Deserialize<WeatherInfo>(json, options);

                Debug.WriteLine("AAAAA prova WI: \n" + weatherInfo);

                return weatherInfo;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Errore nella richiesta del meteo: {ex.Message}");
                return null;
            }
        }
    }
}
