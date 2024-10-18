using System.Diagnostics;
using System.Runtime.CompilerServices;

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

    private void OnListItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem != null)
        {
            Entry entry = e.SelectedItem as Entry;

            var navigationParameter = new Dictionary<string, object>
            {
                { "Entry", entry }
            };

            Shell.Current.GoToAsync($"entrydetails", navigationParameter);
        }
    }

    private void OnItemAdded(object sender, EventArgs e)
    {
        _ = ShowPrompt();
    }

    private async Task ShowPrompt()
    {
        await DisplayAlert("Add City", "To Be Implemented", "OK");
    }

    private async void OnTestPageClicked(object sender, EventArgs e)
    {
        // Naviga alla pagina TestPage
        await Shell.Current.GoToAsync(nameof(TestPage));
    }

    public void testMessage()
    {
        Debug.WriteLine("AAAAAAAAAAAA");
    }

    public void GetCurrentLocation()
    {
        geoLocationService.GetCurrentLocation(BindingContext as BaseViewModel);
    }
}