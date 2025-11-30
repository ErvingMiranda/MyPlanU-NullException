using System.Diagnostics;

namespace MyPlanU.Backend.Services;

public class PromptyLauncher
{
    private const string SCRIPTS_FOLDER = "scripts";

    public async Task StartPromptyApiAsync()
    {
        string scriptName = "start_prompty_api.bat";
        await RunScriptAsync(scriptName);
    }

    public async Task StartPromptyGuiAsync()
    {
        string scriptName = "start_prompty_gui.bat";
        await RunScriptAsync(scriptName);
    }

    private Task RunScriptAsync(string scriptName)
    {
        return Task.Run(() =>
        {
            string promptyBasePath = DetectPromptyBasePath();
            string fullPath = Path.Combine(promptyBasePath, SCRIPTS_FOLDER, scriptName);

            try
            {
                // Verificación básica (opcional, ya que UseShellExecute=true maneja rutas, pero es bueno saber si existe)
                // Nota: File.Exists puede fallar si el path es relativo o complejo, pero aquí usamos absoluto.
                if (!File.Exists(fullPath))
                {
                    // Log o Console
                    Console.WriteLine($"[PromptyLauncher] ADVERTENCIA: No se encontró el script en: {fullPath}");
                    // Intentamos ejecutarlo de todas formas por si el sistema lo resuelve, o lanzamos excepción.
                    // throw new FileNotFoundException("Script no encontrado", fullPath);
                }

                var psi = new ProcessStartInfo
                {
                    FileName = fullPath,
                    UseShellExecute = true,
                    CreateNoWindow = false, // Permitir ventana visible
                    WorkingDirectory = Path.GetDirectoryName(fullPath) // Establecer directorio de trabajo
                };

                Console.WriteLine($"[PromptyLauncher] Iniciando proceso: {fullPath}");
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PromptyLauncher] Error al iniciar {scriptName}: {ex.Message}");
                throw; // Re-lanzar para manejo en UI
            }
        });
    }

    private static string DetectPromptyBasePath()
    {
        string currentDir = AppContext.BaseDirectory;
        DirectoryInfo? dirInfo = new DirectoryInfo(currentDir);

        // Subir hasta 10 niveles buscando "PROMPTY_3.0"
        for (int i = 0; i < 10; i++)
        {
            if (dirInfo == null) break;

            string potentialPath = Path.Combine(dirInfo.FullName, "PROMPTY_3.0");
            string scriptCheckPath = Path.Combine(potentialPath, "scripts", "start_prompty_api.bat");

            if (Directory.Exists(potentialPath) && File.Exists(scriptCheckPath))
            {
                return potentialPath;
            }

            dirInfo = dirInfo.Parent;
        }

        throw new DirectoryNotFoundException("No se encontró la carpeta PROMPTY_3.0 con los scripts necesarios en los directorios superiores. Asegúrate de que la carpeta PROMPTY_3.0 esté al mismo nivel que la solución.");
    }
}
