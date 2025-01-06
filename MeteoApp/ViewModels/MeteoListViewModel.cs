using System.Collections.ObjectModel;
using System.Diagnostics;

namespace MeteoApp
{
    /**
     * Classe ViewModel che fa da base alla lista di locazioni
     */
    public class MeteoListViewModel : BaseViewModel
    {
        private readonly MyDatabase _database;

        /**
         * location aggiunte tramite bottone add
         * */
        private ObservableCollection<Entry> _entries;

        private Entry _currentLocation;
        private ObservableCollection<Entry> _filteredEntries;

        // Tutte le locazioni
        public ObservableCollection<Entry> Entries
        {
            get => _entries;
            set
            {
                if (_entries != value)
                {
                    _entries = new ObservableCollection<Entry>(value.OrderByDescending(e => e.IsCurrentLocation));
                    CurrentLocation = _entries.FirstOrDefault();
                    FilteredEntries = new ObservableCollection<Entry>(_entries.Skip(1));
                    OnPropertyChanged(nameof(Entries)); // Notifica il cambiamento di Entries
                }
            }
        }

        // Locazioni escluse quella corrente
        public ObservableCollection<Entry> FilteredEntries
        {
            get => _filteredEntries;
            set
            {
                if (_filteredEntries != value)
                {
                    _filteredEntries = value;
                    OnPropertyChanged(nameof(FilteredEntries)); // Notifica il cambiamento di FilteredEntries
                }
            }
        }

        // Locazione corrente
        public Entry CurrentLocation
        {
            get => _currentLocation;
            set
            {
                if (_currentLocation != value)
                {
                    _currentLocation = value;
                    OnPropertyChanged(nameof(CurrentLocation)); // Notifica il cambiamento di CurrentLocation
                }
            }
        }

        public MeteoListViewModel(MyDatabase database)
        {
            _database = database;

            // Inizializza le entries dal database
            var dbEntries = _database.GetEntries();
            _entries = new ObservableCollection<Entry>(dbEntries.OrderByDescending(e => e.IsCurrentLocation));
            CurrentLocation = _entries.FirstOrDefault();
            FilteredEntries = new ObservableCollection<Entry>(_entries.Skip(1));
        }

        // Metodo per aggiornare le entries ogni volta che la pagina viene visualizzata
        public void RefreshEntries()
        {
            var newEntries = _database.GetEntries();

            if (!newEntries.SequenceEqual(_entries)) // Confronta gli elementi
            {
                Debug.WriteLine("Aggiornamento delle entries.");
                Entries = new ObservableCollection<Entry>(newEntries.OrderByDescending(e => e.IsCurrentLocation));
            }
        }
    }
}
