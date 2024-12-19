using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using MeteoApp.Services;

namespace MeteoApp;

/**
 *  Questa classe eredita da Shell, che è una componente di navigazione in .NET MAUI.
 */
public partial class MeteoListPage : Shell
{
    private readonly GeoLocationService _geoLocationService;
    private readonly MeteoListViewModel _viewModel;
    private readonly MyDatabase _database;

    public Dictionary<string, Type> Routes { get; private set; } = new Dictionary<string, Type>();

    public MeteoListPage(MyDatabase myDatabase, GeoLocationService geoLocationService)
	{
		InitializeComponent();

        RegisterRoutes();
        _geoLocationService = geoLocationService;
        _database = myDatabase;

        /*
         * Qualsiasi operazione di Binding nel contesto di questa pagina farà
         * riferimento a MeteoListViewModel, in questo caso le entries
         */
        _viewModel = new MeteoListViewModel(myDatabase);
        BindingContext = _viewModel; // Imposta il ViewModel come BindingContext

        // Aggiungi l'evento Navigated di Shell
        this.Navigated += OnNavigated;
    }

    // Metodo che verrà chiamato quando la pagina è attiva dopo la navigazione
    private void OnNavigated(object sender, ShellNavigatedEventArgs e)
    {
        // Verifica se la pagina corrente è MeteoListPage confrontando l'istanza, metodo poco ortodosso lo so
        // ma é l'unico che funziona
        if(Shell.Current.CurrentPage.GetType().FullName.Equals("Microsoft.Maui.Controls.ContentPage"))
        {
            ReloadEntries();
        }
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
        if (e.CurrentSelection.FirstOrDefault() != null)
        {
            Entry entry = e.CurrentSelection.FirstOrDefault() as Entry;

            var navigationParameter = new Dictionary<string, object>
        {
            { "Entry", entry }
        };

            Shell.Current.GoToAsync($"entrydetails", navigationParameter);
        }

    // Deseleziona l'elemento per consentire una nuova selezione
    ((CollectionView)sender).SelectedItem = null;
    }
    /**
     * A questo metodo va aggiunta la mappa di google maps
     */
    private async void OnItemAdded(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(MapPage));
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

    public async void GetCurrentLocation()
    {
        await _geoLocationService.GetCurrentLocation(BindingContext as BaseViewModel);
    }

    private async void ReloadEntries()
    {
        await _geoLocationService.GetCurrentLocation(_viewModel);
        _viewModel.RefreshEntries();
    }
    /**
    * Bug da fixare: se viene eliminate la CurrentLocation
    * Bisogna ricaricarla
    */
    private void OnDeleteButtonClicked(object sender, EventArgs e)
    {
        // Ottiene l'entry associata al pulsante
        var button = sender as Button;
        var entryToDelete = button?.CommandParameter as Entry;

        if (entryToDelete != null && !entryToDelete.IsCurrentLocation)
        {
            // Rimuove l'entry dal database e dall'ObservableCollection
            // Funziona ma non si aggiorna la observable collection
            _database.Remove(entryToDelete);
            ReloadEntries();
        }
        else if (entryToDelete.IsCurrentLocation)
        {
            DisplayAlert("Errore", "Non puoi eliminare la CurrentLocation", "Ok");
        }
    }
}