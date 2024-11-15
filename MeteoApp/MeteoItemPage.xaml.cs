using MeteoApp.Services;
using MeteoApp.ViewModels;

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
}
