using MeteoApp.Services;
using MeteoApp.ViewModels;
using System.Diagnostics;

namespace MeteoApp;

[QueryProperty(nameof(Entry), "Entry")]
public partial class MeteoItemPage : ContentPage
{
    private readonly MeteoItemViewModel _viewModel;

    public Entry Entry
    {
        get => _viewModel.Entry;
        set => _viewModel.Entry = value;
    }

    public MeteoItemPage(WeatherService weatherService)
    {
        InitializeComponent();

        _viewModel = new MeteoItemViewModel(weatherService);
        BindingContext = _viewModel;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();

        await _viewModel.LoadWeatherInfoAsync();
    }

    private async void OnCompleteAddressClicked(object sender, EventArgs e)
    {
        // Navigazione alla pagina Blazor
        await Navigation.PushAsync(new WeatherBlazorPage(GetMeteoItemWeatherInfo()));
    }

    public WeatherInfo GetMeteoItemWeatherInfo()
    {
        return _viewModel.WeatherInfo;
    }
}
