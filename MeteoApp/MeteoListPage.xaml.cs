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

        /**
         * Qualsiasi operazione di Binding nel contesto di questa pagina farà
         * riferimento a MeteoListViewModel, in questo caso le entries
         */
        BindingContext = new MeteoListViewModel();
    }

    private void RegisterRoutes()
    {
        Routes.Add("entrydetails", typeof(MeteoItemPage));
        Routes.Add(nameof(TestPage), typeof(TestPage)); // Aggiungi la nuova pagina TestPage

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

    private void OnItemAdded(object sender, EventArgs e)
    {
        //_ = ShowPrompt();
        _ = AddPersonalLocation();
    }

    private async Task ShowPrompt()
    {
        await DisplayAlert("Add City", "To Be Implemented", "OK");
    }

    private async Task AddPersonalLocation()
    {
        MeteoListViewModel meteoListViewModel = BindingContext as MeteoListViewModel;
        List<Entry> entries = App.Database.GetEntries();

        if (entries.Count == 1) //non ci sono personalLocations, ne crea di nuove
        {

            Entry personalEntry1 = new Entry
            {
                Id = 2,
                CompleteAddress = "123 Main St, Anytown, AT 12345",
                Street = "Main St",
                City = "Anytown",
                PostalCode = "12345",
                Country = "AT"
            };

            var personalEntry2 = new Entry
            {
                Id = 3,
                CompleteAddress = "456 Elm St, Springville, SP 54321",
                Street = "Elm St",
                City = "Springville",
                PostalCode = "54321",
                Country = "SP"
            };

            var personalEntry3 = new Entry
            {
                Id = 4,
                CompleteAddress = "789 Oak Ave, Maple City, MC 67890",
                Street = "Oak Ave",
                City = "Maple City",
                PostalCode = "67890",
                Country = "MC"
            };

            geoLocationService.AddToDBPersonalLocation(BindingContext as BaseViewModel, personalEntry1);
            geoLocationService.AddToDBPersonalLocation(BindingContext as BaseViewModel, personalEntry2);
            geoLocationService.AddToDBPersonalLocation(BindingContext as BaseViewModel, personalEntry3);
        }            
    }

    private async void OnTestPageClicked(object sender, EventArgs e)
    {
        // Naviga alla pagina TestPage
        await Shell.Current.GoToAsync(nameof(TestPage));
    }

    public void GetCurrentLocation()
    {
        geoLocationService.GetCurrentLocation(BindingContext as BaseViewModel);
    }
}