using MeteoApp.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace MeteoApp
{
    /**
     * Classe ViewModel che fa da base alla lista di locazioni
     */
    public class MeteoListViewModel : BaseViewModel
    {
        /**
         * location aggiunte tramite bottone add
         * */
        ObservableCollection<Entry> _entries; 
        /**
         * Location corrente, acquisita ogni volta che si apre l'app (una volta sola).
         * */
        CurrentLocationEntry _currentLocation; 

        public ObservableCollection<Entry> Entries
        {
            get { return _entries; }
            set
            {
                _entries = value;
                OnPropertyChanged();
            }
        }

        public CurrentLocationEntry CurrentLocation { 
            get { return _currentLocation; } 
            set 
            {
                _currentLocation = value;
                OnPropertyChanged();
                Debug.WriteLine("AAAAAA NUOVO VALORE DI CURRENTLOCATION " + value.CompleteAddress);
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

            Entries.Add(new Entry
            {
                Id = 2, // In un contesto reale, l'Id potrebbe essere generato automaticamente dal database
                CompleteAddress = "123 Main Street, Springfield, 12345, USA",
                Street = "123 Main Street",
                City = "Springfield",
                PostalCode = "12345",
                Country = "USA"
            });
        }
    }
}
