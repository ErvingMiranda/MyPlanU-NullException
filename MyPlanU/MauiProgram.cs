using Microsoft.Extensions.Logging;
using MyPlanU.Data;
using MyPlanU.Data.Repositories;
using System.Diagnostics;
using System.IO;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;
using MyPlanU.Services;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;


namespace MyPlanU
{
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
                });

            // Inicializa SQLite (recomendado por sqlite-net-pcl)
            try
            {
                SQLitePCL.Batteries_V2.Init();
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine($"SQLite init error: {ex}");
            }

            // Registro de AppDatabase como singleton
            builder.Services.AddSingleton<AppDatabase>(sp =>
            {
                var dbPath = Path.Combine(FileSystem.AppDataDirectory, "myplanu.db3");

                // Asegura que el directorio exista
                Directory.CreateDirectory(FileSystem.AppDataDirectory);

                // Logs útiles para localizar el archivo
                Debug.WriteLine($"PACKAGE => {AppInfo.PackageName}");
                Debug.WriteLine($"APP DATA => {FileSystem.AppDataDirectory}");
                Debug.WriteLine($"DB PATH  => {dbPath}");

                AppDatabase db;
                try
                {
                    db = new AppDatabase(dbPath);
                    db.InitAsync().GetAwaiter().GetResult();
                }
                catch (System.Exception ex)
                {
                    Debug.WriteLine($"DB INIT ERROR => {ex}");
                    throw;
                }

                // Verificación explícita y listado de archivos si no existe
                var exists = File.Exists(dbPath);
                Debug.WriteLine($"DB EXISTS => {exists}");
                if (!exists)
                {
                    try
                    {
                        var files = string.Join(", ", Directory.GetFiles(FileSystem.AppDataDirectory));
                        Debug.WriteLine($"APP DATA FILES => {files}");
                    }
                    catch (System.Exception listEx)
                    {
                        Debug.WriteLine($"LIST FILES ERROR => {listEx}");
                    }
                }

                return db;
            });

            // Registro de repositorios
            builder.Services.AddSingleton<IUsuarioRepository, UsuarioRepository>();
            builder.Services.AddSingleton<IActividadRepository, ActividadRepository>();
            builder.Services.AddSingleton<IRecordatorioRepository, RecordatorioRepository>();
            builder.Services.AddSingleton<IAmistadRepository, AmistadRepository>();
            builder.Services.AddSingleton<IPromptyConfigRepository, PromptyConfigRepository>();
            builder.Services.AddSingleton<IActividadCompartidaRepository, ActividadCompartidaRepository>();

            // Registro de cliente HTTP para PROMPTY (FastAPI en localhost:8000)
            builder.Services.AddHttpClient<IPromptyClient, PromptyClient>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:8000");
            });

            // Registro de páginas que usan DI
            builder.Services.AddTransient<PromptyPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
