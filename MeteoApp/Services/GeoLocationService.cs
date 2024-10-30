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
    private static GeoLocationService _instance;
    public static GeoLocationService Instance => _instance ??= App.Services.GetRequiredService<GeoLocationService>();

    public async Task GetCurrentLocation(BaseViewModel bindingContext)
    {
        try
        {
            (bindingContext as MeteoListViewModel).IsBusy = true; // Inizia il caricamento
            var permissions = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            GeolocationRequest locationRequest;
            Location location = null;

            if (permissions == PermissionStatus.Granted)
            {
                locationRequest = new GeolocationRequest(GeolocationAccuracy.Best);
                location = await Geolocation.GetLocationAsync(locationRequest);

                AddToDB(bindingContext, await ReverseGeoCoding(location));
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Permissions Error", "You have not granted the app permission to access your location.", "OK");

                var requested = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

                if (requested == PermissionStatus.Granted)
                {
                    locationRequest = new GeolocationRequest(GeolocationAccuracy.Best);
                    location = await Geolocation.GetLocationAsync(locationRequest);

                    AddToDB(bindingContext, await ReverseGeoCoding(location));
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
        finally
        {
            (bindingContext as MeteoListViewModel).IsBusy = false; // Fine del caricamento
        }
    }

    public async static Task<Entry> ReverseGeoCoding(Location location, int timeoutMilliseconds = 50000)
    {        
        using (var cts = new CancellationTokenSource(timeoutMilliseconds))
        {
            var placemarks = await Geocoding.GetPlacemarksAsync(location.Latitude, location.Longitude).WaitAsync(cts.Token);
            var placemark = placemarks?.FirstOrDefault();

            if (placemark != null)
            {
                string completeAddress = $"{placemark.Thoroughfare}, {placemark.Locality}, {placemark.PostalCode}, {placemark.CountryName}";
                string street = placemark.Thoroughfare;
                string city = placemark.Locality;
                string postalCode = placemark.PostalCode;
                string country = placemark.CountryName;

                await Application.Current.MainPage.DisplayAlert("Location Address", $"Address: {completeAddress}", "OK");

                return new Entry
                {
                    Id = App.Database.GetCurrentLocationId(),
                    CompleteAddress = completeAddress,
                    Street = street,
                    City = city,
                    PostalCode = postalCode,
                    Country = country
                };
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Location Address", "Non é stato possibile determinare l'address", "OK");
                return null;
            }
        }
    }

    /**
     * La currentLocation deve sostituire quella già presente nel DB
     * E in più deve sostituire il primo elemento della Observable
     * Collection di MeteoListViewModel (passata come parametro al metodo)
     * ATTENZIONE! L'observable collection deve essere sostituita TOTALMENTE,
     * non basta fare add(entry) perché altrimenti non si attiva OnPropertyChange()
     */
    private void AddToDB(BaseViewModel bindingContext, Entry currentLocationEntry)
    {
        //Ok
        App.Database.UpsertCurrentLocation(currentLocationEntry);

        MeteoListViewModel meteoListViewModelContext = bindingContext as MeteoListViewModel;

        // Crea una nuova collezione e sostituisci la vecchia
        ObservableCollection<Entry> newEntries = new ObservableCollection<Entry>(meteoListViewModelContext.Entries);

        // Rimuovi la vecchia currentLocation (se esiste) e aggiungi la nuova
        var previousCurrentLocation = newEntries.FirstOrDefault();
        if (previousCurrentLocation != null)
        {
            newEntries.Remove(previousCurrentLocation);
        }
        newEntries.Insert(0, currentLocationEntry); // Inserisci la nuova CurrentLocation in cima

        // Sostituisci la collezione e chiama OnPropertyChanged
        meteoListViewModelContext.Entries = newEntries;
    }

    public void AddToDBPersonalLocation(BaseViewModel bindingContext, Entry personalEntry)
    {
        MeteoListViewModel meteoListViewModelContext = bindingContext as MeteoListViewModel;

        meteoListViewModelContext.Entries.Add(personalEntry);

        App.Database.SaveEntry(personalEntry);          
    }
}
