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
    private Timer _timer;
    private BaseViewModel _bindingContext;
    private int _UpdateCurrentLocationTimer = 20; //variabile da modificare in base al periodo di aggiornamento desiderato

    public GeoLocationService(MyDatabase database)
    {
        _database = database;

        // Imposta il timer per eseguire la funzione ogni tot minuti
        _timer = new Timer(async (e) =>
        {
            await GetCurrentLocation();
        }, null, TimeSpan.Zero, TimeSpan.FromSeconds(_UpdateCurrentLocationTimer));
    }

    public async Task GetCurrentLocation()
    {
        Debug.WriteLine("TIMERRR XXXXXXXXXXXXXXXXXXXXXXXXXXXXXxxxx");
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
        _database.UpsertCurrentLocation(currentLocationEntry);

        MeteoListViewModel meteoListViewModelContext = _bindingContext as MeteoListViewModel;

        // Crea una nuova collezione e sostituisci la vecchia
        ObservableCollection<Entry> newEntries = new ObservableCollection<Entry>(meteoListViewModelContext.Entries);

        // Rimuovi la vecchia currentLocation (se esiste) e aggiungi la nuova
        var previousCurrentLocation = newEntries.FirstOrDefault<Entry>(e => e.IsCurrentLocation);
        if (previousCurrentLocation != null)
        {
            newEntries.Remove(previousCurrentLocation);
        }
        newEntries.Insert(0, currentLocationEntry); // Inserisci la nuova CurrentLocation in cima

        // Sostituisci la collezione e chiama OnPropertyChanged
        meteoListViewModelContext.Entries = newEntries;
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
