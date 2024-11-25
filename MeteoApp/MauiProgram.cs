using Microsoft.Extensions.Logging;
using MeteoApp.Services;
using System.Reflection;
using Microsoft.Extensions.Configuration;

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
			.UseMauiMaps();

        //Aggiungere il file appsettings.json, contenente l'api key di openweather alla configurazione
        //dell'app:
        var assembly = Assembly.GetExecutingAssembly();

        var resourcePrefix = "MeteoApp.Resources.ApiKey.";
        var resourceName = $"{resourcePrefix}appsettings.json";

        using var stream = assembly.GetManifestResourceStream(resourceName);

        if (stream == null)
        {
            throw new Exception($"Impossibile trovare la risorsa {resourceName}");
        }

        // Costruisci la configurazione
        var config = new ConfigurationBuilder()
            .AddJsonStream(stream)
            .Build();

        // Aggiungi la configurazione al builder
        builder.Configuration.AddConfiguration(config);

        // Registra WeatherService come servizio singleton
        builder.Services.AddSingleton<WeatherService>();
        builder.Services.AddTransient<MeteoItemPage>();

        // Registra il servizio come singleton
        // Le funzioni di GeoLocation servono in tutto il codice, renderlo un singleton é 
        // l'approccio più semplice ed efficiente. Idem per Database
        builder.Services.AddSingleton<GeoLocationService>();
		// Registra MyDatabase come singleton per DI
		builder.Services.AddSingleton<MyDatabase>();
		// Registra le classi che hanno bisogno di Dependency Injection
		builder.Services.AddTransient<MeteoListPage>();
		builder.Services.AddTransient<MeteoListViewModel>();
		builder.Services.AddTransient<MapPage>();

        builder.Services.AddTransient<WeatherBlazorPage>();

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif
        return builder.Build();
	}
}

