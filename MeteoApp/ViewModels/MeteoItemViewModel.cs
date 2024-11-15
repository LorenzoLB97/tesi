using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MeteoApp.Services;

namespace MeteoApp.ViewModels
{
    public class MeteoItemViewModel : BaseViewModel
    {
        private readonly WeatherService _weatherService;

        private Entry entry;
        private WeatherInfo weatherInfo;

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
            get => weatherInfo;
            set
            {
                weatherInfo = value;
                OnPropertyChanged();
            }
        }

        public MeteoItemViewModel(WeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        public async Task LoadWeatherInfoAsync()
        {
            if (Entry != null)
            {
                WeatherInfo = await _weatherService.GetCurrentWeatherAsync(Entry);
            }
        }
    }
}
