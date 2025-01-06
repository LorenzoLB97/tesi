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
    private bool _isSwipeInProgress = false; // Flag per gestire lo swipe

    public Dictionary<string, Type> Routes { get; private set; } = new Dictionary<string, Type>();

    public MeteoListPage(MyDatabase myDatabase, GeoLocationService geoLocationService)
    {
        InitializeComponent();

        RegisterRoutes();
        _database = myDatabase;

        /*
         * Qualsiasi operazione di Binding nel contesto di questa pagina farà
         * riferimento a MeteoListViewModel, in questo caso le entries
         */
        _viewModel = new MeteoListViewModel(myDatabase);
        BindingContext = _viewModel; // Imposta il ViewModel come BindingContext

        geoLocationService.SetBindingContext(_viewModel);
        _geoLocationService = geoLocationService;

        // Aggiungi l'evento Navigated di Shell
        this.Navigated += OnNavigated;
    }

    // Metodo che verrà chiamato quando la pagina è attiva dopo la navigazione
    private void OnNavigated(object sender, ShellNavigatedEventArgs e)
    {
        // Verifica se la pagina corrente è MeteoListPage confrontando l'istanza
        if (Shell.Current.CurrentPage.GetType().FullName.Equals("Microsoft.Maui.Controls.ContentPage"))
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
        if (_isSwipeInProgress)
        {
            // Ignora l'evento di selezione se uno swipe è in corso
            _isSwipeInProgress = false;
            return;
        }

        if (e.CurrentSelection.FirstOrDefault() != null)
        {
            Entry entry = e.CurrentSelection.FirstOrDefault() as Entry;

            var navigationParameter = new Dictionary<string, object>
            {
                { "Entry", entry }
            };

            Shell.Current.GoToAsync($"entrydetails", navigationParameter);
        }

        // Deseleziona l'elemento per consentire una nuova selezione in futuro
        ((CollectionView)sender).SelectedItem = null;
    }

    /**
     * A questo metodo va aggiunta la mappa di google maps
     */
    private async void OnItemAdded(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(MapPage));
    }

    private async void OnTestPageClicked(object sender, EventArgs e)
    {
        // Naviga alla pagina TestPage
        await Shell.Current.GoToAsync(nameof(TestPage));
    }

    public async void GetCurrentLocation()
    {
        await _geoLocationService.GetCurrentLocation();
    }

    private async void ReloadEntries()
    {
        await _geoLocationService.GetCurrentLocation();
        _viewModel.RefreshEntries();
    }

    private void OnSwipeEnded(object sender, EventArgs e)
    {
        _isSwipeInProgress = true; // Imposta il flag per ignorare la selezione

        Debug.WriteLine("SWIPE");
        var swipeItem = sender as SwipeItem;
        var entryToDelete = swipeItem?.CommandParameter as Entry;

        if (entryToDelete == null)
            return;

        if (entryToDelete.IsCurrentLocation)
        {
            DisplayAlert("Errore", "Non puoi eliminare la CurrentLocation", "Ok");
            return;
        }

        // Rimuovi l'entry dal database
        _database.Remove(entryToDelete);

        // Rimuovi l'entry dalla ObservableCollection
        if (BindingContext is MeteoListViewModel viewModel)
        {
            viewModel.Entries.Remove(entryToDelete);
        }

        _isSwipeInProgress = false; // Resetta il flag
    }

    private void OnDeleteItemBySwipe(object sender, EventArgs e)
    {
        _isSwipeInProgress = true; // Imposta il flag per ignorare la selezione

        if (sender is SwipeItem swipeItem && swipeItem.CommandParameter is Entry entryToDelete)
        {
            if (entryToDelete.IsCurrentLocation)
            {
                DisplayAlert("Errore", "Non puoi eliminare la CurrentLocation", "Ok");
                return;
            }

            // Rimuovi l'entry dal database
            _database.Remove(entryToDelete);

            // Rimuovi l'entry dalla ObservableCollection
            _viewModel.Entries.Remove(entryToDelete);
        }

        _isSwipeInProgress = false; // Resetta il flag
    }

    // Add this method
    private void OnItemTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is Entry entry)
        {
            var navigationParameter = new Dictionary<string, object>
        {
            { "Entry", entry }
        };

            Shell.Current.GoToAsync($"entrydetails", navigationParameter);
        }
    }

}
