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
        /**
         * Location corrente, acquisita ogni volta che si apre l'app (una volta sola).
         * */

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
    }
}
