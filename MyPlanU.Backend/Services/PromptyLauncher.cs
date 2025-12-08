using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace MyPlanU.Backend.Services;

public class PromptyLauncher
{
    private const string SCRIPTS_FOLDER = "scripts";

    public async Task StartPromptyApiAsync()
    {
        string scriptName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? "start_prompty_api.bat"
            : "start_prompty_api.sh";

        await RunScriptAsync(scriptName, waitForExit: false);
    }

    public async Task StartPromptyGuiAsync()
    {
        string scriptName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? "start_prompty_gui.bat"
            : "start_prompty_gui.sh"; // Asumiendo que existe o se creará

        await RunScriptAsync(scriptName, waitForExit: true);
    }

    private async Task RunScriptAsync(string scriptName, bool waitForExit)
    {
        string promptyBasePath = DetectPromptyBasePath();
        string fullPath = Path.Combine(promptyBasePath, SCRIPTS_FOLDER, scriptName);

        if (!File.Exists(fullPath))
        {
            string errorMsg = $"[PromptyLauncher] ERROR CRÍTICO: No se encontró el script en: {fullPath}";
            Console.WriteLine(errorMsg);
            throw new FileNotFoundException(errorMsg, fullPath);
        }

        var process = new Process();
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                process.StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c \"{fullPath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = Path.GetDirectoryName(fullPath) ?? promptyBasePath
                };
            }
            else
            {
                // Asegurar permisos de ejecución en UNIX
                try { File.SetUnixFileMode(fullPath, UnixFileMode.UserExecute | UnixFileMode.UserRead | UnixFileMode.UserWrite); } catch { /* Ignorar si falla */ }
                process.StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"'{fullPath}'\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = Path.GetDirectoryName(fullPath) ?? promptyBasePath
                };
            }

            var outputBuilder = new StringBuilder();
            var errorBuilder = new StringBuilder();

            process.OutputDataReceived += (sender, args) => { if (args.Data != null) outputBuilder.AppendLine(args.Data); };
            process.ErrorDataReceived += (sender, args) => { if (args.Data != null) errorBuilder.AppendLine(args.Data); };

            Console.WriteLine($"[PromptyLauncher] Iniciando proceso: {process.StartInfo.FileName} {process.StartInfo.Arguments}");
            process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            if (waitForExit)
            {
                await process.WaitForExitAsync();
            }
            else
            {
                // No esperamos a que el proceso termine (await process.WaitForExitAsync())
                // porque es un servidor que debe quedar corriendo en segundo plano.
                // Damos un pequeño margen para capturar errores de arranque inmediato.
                await Task.Delay(2000);

                if (process.HasExited)
                {
                    string errorOutput = errorBuilder.ToString();
                    if (process.ExitCode != 0 || !string.IsNullOrWhiteSpace(errorOutput))
                    {
                        throw new InvalidOperationException(
                            $"[PromptyLauncher] El script '{scriptName}' falló al iniciar con código {process.ExitCode}. " +
                            $"Error: {errorOutput}. " +
                            $"Salida: {outputBuilder.ToString()}");
                    }
                }
            }

            Console.WriteLine($"[PromptyLauncher] El proceso para '{scriptName}' se ha iniciado correctamente en segundo plano (PID: {process.Id}).");

        }
        catch (Exception ex)
        {
            Console.WriteLine($"[PromptyLauncher] Excepción al iniciar {scriptName}: {ex.Message}");
            throw;
        }
        // No hacemos 'dispose' del proceso si queremos que siga corriendo.
        // Será responsabilidad del sistema operativo gestionarlo.
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