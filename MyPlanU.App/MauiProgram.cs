using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using MyPlanU.Backend.Business;
using MyPlanU.Backend.Data;
using MyPlanU.Backend.Data.Repositories;
using MyPlanU.Backend.Services;
using MyPlanU.App.Pages;
using MyPlanU.App.ViewModels;
using Microsoft.Maui.Storage;

namespace MyPlanU.App;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

        // Services
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "MyPlanU.db3");
        builder.Services.AddSingleton<SQLiteContext>(s => new SQLiteContext(dbPath));
        builder.Services.AddSingleton<IUsuarioRepository, UsuarioRepository>();
        builder.Services.AddSingleton<IActividadRepository, ActividadRepository>();
        builder.Services.AddSingleton<IRecordatorioRepository, RecordatorioRepository>();
        builder.Services.AddSingleton<IActividadCompartidaRepository, ActividadCompartidaRepository>();

        builder.Services.AddSingleton<AuthService>();
        builder.Services.AddSingleton<ActividadService>();
        builder.Services.AddSingleton<RecordatorioService>();
        builder.Services.AddSingleton<ActividadCompartidaService>();

        // Prompty Client
        builder.Services.AddHttpClient<IPromptyClient, PromptyClient>(client =>
        {
            // Ajustar URL base según entorno (localhost para Windows, 10.0.2.2 para Android Emulator)
            string baseUrl = DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:8000" : "http://localhost:8000";
            client.BaseAddress = new Uri(baseUrl);
        });

        // Pages & ViewModels
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<LoginViewModel>();

        builder.Services.AddTransient<RegistroPage>();
        builder.Services.AddTransient<RegistroViewModel>();

        builder.Services.AddTransient<EventosPage>();
        builder.Services.AddTransient<EventosViewModel>();

        builder.Services.AddTransient<CrearEventoPage>();
        builder.Services.AddTransient<CrearEventoViewModel>();

        builder.Services.AddTransient<ConfiguracionPage>();
        builder.Services.AddTransient<ConfiguracionViewModel>();

        builder.Services.AddTransient<PromptyPage>();
        builder.Services.AddTransient<PromptyViewModel>();

		return builder.Build();
	}
}
