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
			.UseMauiMaps();			

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

#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder.Build();
	}
}

