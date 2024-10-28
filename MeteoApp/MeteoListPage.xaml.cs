using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using MeteoApp.service;

namespace MeteoApp;

/**
 *  Questa classe eredita da Shell, che è una componente di navigazione in .NET MAUI.
 */
public partial class MeteoListPage : Shell
{
    private readonly GeoLocationService geoLocationService = new GeoLocationService();
    public Dictionary<string, Type> Routes { get; private set; } = new Dictionary<string, Type>();

    public MeteoListPage()
	{
		InitializeComponent();
        RegisterRoutes();

        /*
         * Qualsiasi operazione di Binding nel contesto di questa pagina farà
         * riferimento a MeteoListViewModel, in questo caso le entries
         */
        BindingContext = new MeteoListViewModel();
    }

    private void RegisterRoutes()
    {
        Routes.Add("entrydetails", typeof(MeteoItemPage));
        Routes.Add(nameof(TestPage), typeof(TestPage)); // Aggiungi la nuova pagina TestPage
        Routes.Add(nameof(MapPage), typeof(MapPage));

        foreach (var item in Routes)
            Routing.RegisterRoute(item.Key, item.Value);
    }

    private void OnListItemSelected(object sender, SelectionChangedEventArgs e)
    {
        Debug.WriteLine("CLICCATO SU ITEM");
        if (e.CurrentSelection.FirstOrDefault() != null)
        {
            Entry entry = e.CurrentSelection.FirstOrDefault() as Entry;

            var navigationParameter = new Dictionary<string, object>
            {
                { "Entry", entry }
            };

            Shell.Current.GoToAsync($"entrydetails", navigationParameter);
        }
    }
    /**
     * A questo metodo va aggiunta la mappa di google maps
     */
    private async void OnItemAdded(object sender, EventArgs e)
    {
        //_ = ShowPrompt();     
        //_ = AddPersonalLocation();
        await Shell.Current.GoToAsync(nameof(MapPage));
    }

    private async Task ShowPrompt()
    {
        await DisplayAlert("Add City", "To Be Implemented", "OK");
    }

    /**
     * Va sostituito con una nuova pagina che apre una mappa di google maps
     */
    private void AddPersonalLocation()
    {
        
    }

    private async void OnTestPageClicked(object sender, EventArgs e)
    {
        // Naviga alla pagina TestPage
        await Shell.Current.GoToAsync(nameof(TestPage));
    }

    public async void GetCurrentLocation()
    {
        await geoLocationService.GetCurrentLocation(BindingContext as BaseViewModel);
    }
}