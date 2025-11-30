using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using MyPlanU.Backend.Business;
using MyPlanU.Backend.Data;
using MyPlanU.Backend.Data.Repositories;
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

        // Pages & ViewModels
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<LoginViewModel>();

        builder.Services.AddTransient<EventosPage>();
        builder.Services.AddTransient<EventosViewModel>();

        builder.Services.AddTransient<ConfiguracionPage>();
        builder.Services.AddTransient<ConfiguracionViewModel>();

		return builder.Build();
	}
}
