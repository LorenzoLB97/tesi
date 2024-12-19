using Microsoft.Extensions.Logging;
using MeteoApp.Services;

namespace MeteoApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .UseMauiMaps(); // Abilita le mappe in .NET MAUI

        // Registrazione dei servizi principali
        builder.Services.AddSingleton<GeoLocationService>(); // Servizio per la gestione della geolocalizzazione
        builder.Services.AddSingleton<MyDatabase>(); // Servizio per la gestione del database locale

        // Registrazione delle pagine e dei ViewModel per Dependency Injection
        builder.Services.AddTransient<MeteoListPage>();
        builder.Services.AddTransient<MeteoListViewModel>();
        builder.Services.AddTransient<MapPage>();

#if DEBUG
        builder.Logging.AddDebug(); // Abilita il logging in modalità debug
#endif

        return builder.Build();
    }
}
