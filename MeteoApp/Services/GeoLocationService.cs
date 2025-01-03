using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeteoApp.Services;

public class GeoLocationService
{
    private readonly MyDatabase _database;
    private readonly Timer _timer;
    private BaseViewModel _bindingContext;
    private readonly int _updateCurrentLocationTimer = 3; //variabile da modificare in base al periodo di aggiornamento desiderato

    public GeoLocationService(MyDatabase database)
    {
        _database = database;

        // Imposta il timer per eseguire la funzione ogni tot minuti
        _timer = new Timer(async (e) =>
        {
            await GetCurrentLocation();
        }, null, TimeSpan.Zero, TimeSpan.FromMinutes(_updateCurrentLocationTimer));
    }

    public async Task GetCurrentLocation()
    {
        Debug.WriteLine("ACQUISIZIONE CURRENTLOCATION");
        try
        {
            var permissions = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            GeolocationRequest locationRequest;
            Location location = null;

            if (permissions == PermissionStatus.Granted)
            {
                locationRequest = new GeolocationRequest(GeolocationAccuracy.Best);
                location = await Geolocation.GetLocationAsync(locationRequest);

                Entry currentLocation = await ReverseGeoCoding(location);
                currentLocation.IsCurrentLocation = true;

                AddToDBCurrentLocation(currentLocation);
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Permissions Error", "You have not granted the app permission to access your location.", "OK");

                var requested = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

                if (requested == PermissionStatus.Granted)
                {
                    locationRequest = new GeolocationRequest(GeolocationAccuracy.Best);
                    location = await Geolocation.GetLocationAsync(locationRequest);

                    Entry currentLocation = await ReverseGeoCoding(location);
                    currentLocation.IsCurrentLocation = true;

                    AddToDBCurrentLocation(currentLocation);
                }
                else
                {
                    if (DeviceInfo.Platform == DevicePlatform.iOS || DeviceInfo.Platform == DevicePlatform.MacCatalyst)
                        await Application.Current.MainPage.DisplayAlert("Location Required", "Location is required to share it. Please enable location for this app in Settings.", "OK");
                    else
                        await Application.Current.MainPage.DisplayAlert("Location Required", "Location is required to share it. We'll ask again next time.", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Errore nell'acquisizione della posizione: {ex.Message}");
        }
    }

    public async Task<Entry> ReverseGeoCoding(Location location)
    {                
        var placemarks = await Geocoding.GetPlacemarksAsync(location.Latitude, location.Longitude);
        var placemark = placemarks?.FirstOrDefault();

        if (placemark != null)
        {
            string completeAddress = $"{placemark.Thoroughfare}, {placemark.Locality}, {placemark.PostalCode}, {placemark.CountryName}";
            string street = placemark.Thoroughfare;
            string city = placemark.Locality;
            string postalCode = placemark.PostalCode;
            string country = placemark.CountryName;
            double latitude = placemark.Location.Latitude;
            double longitude = placemark.Location.Longitude;

            return new Entry
            {
                Id = _database.GetCurrentLocationId(),
                CompleteAddress = completeAddress,
                Street = street,
                City = city,
                PostalCode = postalCode,
                Country = country,
                Latitude = latitude,
                Longitude = longitude
            };
        }
        else
        {
            await Application.Current.MainPage.DisplayAlert("Location Address", "Non é stato possibile determinare l'address", "OK");
            return null;
        }        
    }

    /**
     * La currentLocation deve sostituire quella già presente nel DB
     * E in più deve sostituire il primo elemento della Observable
     * Collection di MeteoListViewModel (passata come parametro al metodo)
     * ATTENZIONE! L'observable collection deve essere sostituita TOTALMENTE,
     * non basta fare add(entry) perché altrimenti non si attiva OnPropertyChange()
     */
    private void AddToDBCurrentLocation(Entry currentLocationEntry)
    {
        //Inserisce o aggiorna con la nuova currentLocation
        //Ok
        _database.UpsertCurrentLocation(currentLocationEntry); //ok, ha aggiornato la entry nel DB

        MeteoListViewModel meteoListViewModelContext = _bindingContext as MeteoListViewModel;

        // Crea una nuova collezione e sostituisci la vecchia
        ObservableCollection<Entry> newEntries = new ObservableCollection<Entry>(meteoListViewModelContext.Entries);

        // Rimuovi la vecchia currentLocation (se esiste) e aggiungi la nuova
        //C'é un bug da qualche parte che duplica la currentLocation, non riesco a trovarlo e per ora lo risolvo cosi.
        var entriesToRemove = newEntries.Where(e => e.IsCurrentLocation).ToList();
        foreach (var entry in entriesToRemove)
        {
            newEntries.Remove(entry);
        }

        // Inserisci la nuova CurrentLocation in cima
        newEntries.Insert(0, currentLocationEntry);

        // Sostituisci la collezione e chiama OnPropertyChanged
        Debug.WriteLine("Prima acquisizione e sostituzione di currentLocation");
        meteoListViewModelContext.Entries = newEntries; //fin qua dovrebbe essere tutto ok.
    }

    public BaseViewModel GetBindingContext()
    {
        return _bindingContext;
    }

    public void SetBindingContext(BaseViewModel bindingContext)
    {
        _bindingContext = bindingContext;
    }
}
