using System.Collections.ObjectModel;
using System.Diagnostics;

namespace MeteoApp
{
    /**
     * Classe ViewModel che fa da base alla lista di locazioni
     */
    public class MeteoListViewModel : BaseViewModel
    {
        private bool _isBusy;
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
                OnPropertyChanged();

                if (value.FirstOrDefault() != null)
                {
                    Debug.WriteLine("VALORE DI CURRENTLOCATION CORRENTE: " + value.FirstOrDefault().CompleteAddress);
                }
            }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        public MeteoListViewModel()
        {
            Entries = new ObservableCollection<Entry>();

            List<Entry> dbEntries = App.Database.GetEntries();

            for (var i = 0; i < dbEntries.Count; i++)
            {
                Entries.Add(dbEntries[i]);
            }
        }

        // Metodo per aggiornare le entries ogni volta che la pagina viene visualizzata
        public void RefreshEntries()
        {
            //Entries.Clear(); // Pulisci la lista attuale
            //LoadEntriesFromDatabase(); // Ricarica dal database
            Debug.WriteLine("XXXXXXXXXXXXXXX Entries aggiornate.");
            List<Entry> newEntries = App.Database.GetEntries();
            if (_entries.Count == newEntries.Count) {
                return;
            }

            if (!_entries.Equals(newEntries))
            {
                CheckEntries(newEntries);
            }
        }

        private void CheckEntries(List<Entry> newEntries)
        {
            for (int i=0; i<_entries.Count; i++)
            {
                if (!newEntries[i].Equals(_entries[i]))
                {
                    _entries = new ObservableCollection<Entry>(newEntries);
                    return;
                }
            }
        }
    }
}
