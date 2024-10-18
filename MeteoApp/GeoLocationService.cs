using MeteoApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeteoApp
{
    public class GeoLocationService
    {
        public async Task GetCurrentLocation(BaseViewModel bindingContext)
        {
            var permissions = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            GeolocationRequest locationRequest;
            Location location = null;

            if (permissions == PermissionStatus.Granted)
            {
                locationRequest = new GeolocationRequest(GeolocationAccuracy.Best);
                location = await Geolocation.GetLocationAsync(locationRequest);

                ReverseGeoCoding(bindingContext, location);
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Permissions Error", "You have not granted the app permission to access your location.", "OK");

                var requested = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

                if (requested == PermissionStatus.Granted)
                {
                    locationRequest = new GeolocationRequest(GeolocationAccuracy.Best);
                    location = await Geolocation.GetLocationAsync(locationRequest);

                    ReverseGeoCoding(bindingContext, location);
                }
                else
                {
                    if (DeviceInfo.Platform == DevicePlatform.iOS || DeviceInfo.Platform == DevicePlatform.MacCatalyst)
                        await App.Current.MainPage.DisplayAlert("Location Required", "Location is required to share it. Please enable location for this app in Settings.", "OK");
                    else
                        await App.Current.MainPage.DisplayAlert("Location Required", "Location is required to share it. We'll ask again next time.", "OK");
                }
            }
        }

        public async Task ReverseGeoCoding(BaseViewModel bindingContext, Location location)
        {
            // Utilizza il Reverse Geocoding per ottenere l'indirizzo
            var placemarks = await Geocoding.GetPlacemarksAsync(location.Latitude, location.Longitude);
            var placemark = placemarks?.FirstOrDefault();

            if (placemark != null)
            {
                Debug.WriteLine("Qui6");
                string completeAddress = $"{placemark.Thoroughfare}, {placemark.Locality}, {placemark.PostalCode}, {placemark.CountryName}";
                string street = placemark.Thoroughfare;
                string city = placemark.Locality;
                string postalCode = placemark.PostalCode;
                string country = placemark.CountryName;                  

                await App.Current.MainPage.DisplayAlert("Location Address", $"Address: {completeAddress}", "OK");

                //AddToDB(new CurrentLocationEntry(completeAddress, street, city, postalCode, country));

                AddToDB(bindingContext, new CurrentLocationEntry
                {
                    Id = 1, // In un contesto reale, l'Id potrebbe essere generato automaticamente dal database
                    CompleteAddress = completeAddress,
                    Street = street,
                    City = city,
                    PostalCode = postalCode,
                    Country = country
                });
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Location Address", "Non é stato possibile determinare l'address", "OK");
            }
        }

        private void AddToDB(BaseViewModel bindingContext, CurrentLocationEntry currentLocationEntry)
        {
            App.Database.UpsertCurrentLocation(currentLocationEntry);
            (bindingContext as MeteoListViewModel).CurrentLocation = currentLocationEntry;


            //Funziona: la current location viene effettivamente scritta nel database,
            //ora bisogna vedere perché non viene mostratrata a schermo
            Debug.WriteLine("AAAAAAAAAAAA " + App.Database.GetCurrentLocationEntry().CompleteAddress);
        }
    }
}
