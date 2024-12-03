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
        ObservableCollection<Entry> _entries; 

        public ObservableCollection<Entry> Entries
        {
            get { return _entries; }
            set
            {
                _entries = value;

                if (value.FirstOrDefault() != null)
                {
                    Debug.WriteLine("(Al primo avvio non é corretto) VALORE DI CURRENTLOCATION CORRENTE: " + value.FirstOrDefault().CompleteAddress);
                }

                // Sort the entries so that currentLocation is first
                var sortedEntries = value.OrderByDescending(e => e.IsCurrentLocation);
                _entries = new ObservableCollection<Entry>(sortedEntries);
                OnPropertyChanged();
            }
        }

        public MeteoListViewModel(MyDatabase database)
        {
            _database = database;
            Entries = new ObservableCollection<Entry>();

            List<Entry> dbEntries = _database.GetEntries();

            for (var i = 0; i < dbEntries.Count; i++)
            {
                Entries.Add(dbEntries[i]);
            }
        }

        // Metodo per aggiornare le entries ogni volta che la pagina viene visualizzata
        public void RefreshEntries()
        {
            List<Entry> newEntries = _database.GetEntries();
            if (_entries.Count == newEntries.Count) {
                return;
            }

            //BUG: quando fa il reloading delle entries perché viene rivisualizzata ListPage
            //aggiunge una nuova currentLocation, anche se esiste già quella precedente.
            if (!_entries.Equals(newEntries))
            {
                Debug.WriteLine("ZZZZZZZZZZZZZZZZZZZz");
                Entries = new ObservableCollection<Entry>(newEntries);                
            }
        }
    }
}
