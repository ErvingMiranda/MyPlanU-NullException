using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MyPlanU.Backend.Services;

public class PromptyLauncher
{
    private const string SCRIPTS_FOLDER = "scripts";

    public async Task StartPromptyApiAsync()
    {
        string scriptName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) 
            ? "start_prompty_api.bat" 
            : "start_prompty_api.sh";
            
        await RunScriptAsync(scriptName);
    }

    public async Task StartPromptyGuiAsync()
    {
        string scriptName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) 
            ? "start_prompty_gui.bat" 
            : "start_prompty_gui.sh"; // Asumiendo que existe o se creará
            
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
                // Verificación básica
                if (!File.Exists(fullPath))
                {
                    Console.WriteLine($"[PromptyLauncher] ADVERTENCIA: No se encontró el script en: {fullPath}");
                }

                var psi = new ProcessStartInfo
                {
                    FileName = fullPath,
                    UseShellExecute = true,
                    CreateNoWindow = false,
                    WorkingDirectory = Path.GetDirectoryName(fullPath)
                };

                // En Linux/Mac, a veces es mejor invocar bash explícitamente si UseShellExecute falla o para asegurar terminal
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    // Asegurar permisos de ejecución (intento best-effort)
                    try 
                    { 
                        File.SetUnixFileMode(fullPath, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute); 
                    } 
                    catch { /* Ignorar si falla o no soportado */ }
                }

                Console.WriteLine($"[PromptyLauncher] Iniciando proceso: {fullPath}");
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PromptyLauncher] Error al iniciar {scriptName}: {ex.Message}");
                throw;
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
