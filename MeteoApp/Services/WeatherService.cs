using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Diagnostics;
using static System.Net.WebRequestMethods;

namespace MeteoApp.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public WeatherService(string apiKey)
        {
            _httpClient = new HttpClient();
            _apiKey = apiKey;
        }

        // Metodo per ottenere il meteo attuale per una determinata città
        public async Task<WeatherInfo> GetCurrentWeatherAsync(Entry entry)
        {

            double lat = entry.Latitude;
            double lon = entry.Longitude;

            Debug.WriteLine("HHHHHKKKKK: " + _apiKey);

            string url = $"https://api.openweathermap.org/data/2.5/forecast?lat={lat}&lon={lon}&appid={_apiKey}";

            try
            {
                Debug.WriteLine("XXXXXXXXXXXXXX Avviata richiesta http");
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode(); // Lancia un'eccezione se lo status code non è 2xx

                var json = await response.Content.ReadAsStringAsync();
                var weatherInfo = JsonSerializer.Deserialize<WeatherInfo>(json);

                Debug.WriteLine($"YYYYYYYYYYYYY Weather Info: {weatherInfo.Wind}");

                return weatherInfo;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore nella richiesta del meteo: {ex.Message}");
                return null;
            }
        }
    }
}
