using System.Collections.ObjectModel;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyPlanU.Backend.Models;
using MyPlanU.Backend.Services;

namespace MyPlanU.App.ViewModels;

public partial class PromptyViewModel : ObservableObject
{
    private readonly IPromptyLiteClient _promptyClient;
    private readonly PromptyLauncher _promptyLauncher;
    
    [ObservableProperty]
    private ObservableCollection<ChatMessage> mensajes = new();

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(EnviarMensajeCommand))]
    private string mensajeEntrada = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(EnviarMensajeCommand))]
    private bool isSending;

    public PromptyViewModel(IPromptyLiteClient promptyClient, PromptyLauncher promptyLauncher)
    {
        _promptyClient = promptyClient;
        _promptyLauncher = promptyLauncher;
    }

    public async Task InitializeAsync()
    {
        // Verificar si la API responde. Si no, intentar levantarla.
        bool isHealthy = await _promptyClient.CheckHealthAsync();
        if (!isHealthy)
        {
            Mensajes.Add(new ChatMessage { Rol = "Sistema", Texto = "La API de PROMPTY no responde. Iniciando servicio..." });
            try
            {
                await _promptyLauncher.StartPromptyApiAsync();
                // Dar un momento para que arranque
                // No bloqueamos demasiado, el usuario puede reintentar si falla el primer mensaje.
            }
            catch (Exception ex)
            {
                Mensajes.Add(new ChatMessage { Rol = "Error", Texto = $"No se pudo iniciar la API: {ex.Message}" });
            }
        }
    }

    [RelayCommand]
    private async Task StartApiAsync()
    {
        try
        {
            await _promptyLauncher.StartPromptyApiAsync();
            Mensajes.Add(new ChatMessage { Rol = "Sistema", Texto = "Iniciando API de PROMPTY..." });
        }
        catch (Exception ex)
        {
            Mensajes.Add(new ChatMessage { Rol = "Error", Texto = $"Error al iniciar API: {ex.Message}" });
        }
    }

    [RelayCommand]
    private async Task StartGuiAsync()
    {
        try
        {
            await _promptyLauncher.StartPromptyGuiAsync();
            Mensajes.Add(new ChatMessage { Rol = "Sistema", Texto = "Iniciando GUI de PROMPTY..." });
        }
        catch (Exception ex)
        {
            Mensajes.Add(new ChatMessage { Rol = "Error", Texto = $"Error al iniciar GUI: {ex.Message}" });
        }
    }

    private bool CanEnviarMensaje()
    {
        return !string.IsNullOrWhiteSpace(MensajeEntrada) && !IsSending;
    }

    [RelayCommand(CanExecute = nameof(CanEnviarMensaje))]
    private async Task EnviarMensajeAsync()
    {
        var texto = MensajeEntrada;
        MensajeEntrada = string.Empty;
        IsSending = true;

        // 1. Añadir mensaje del usuario
        Mensajes.Add(new ChatMessage { Rol = "Usuario", Texto = texto });

        // Asegurar que la API esté corriendo
        if (!await _promptyClient.CheckHealthAsync())
        {
             Mensajes.Add(new ChatMessage { Rol = "Sistema", Texto = "API no detectada. Intentando iniciar..." });
             try 
             {
                 await _promptyLauncher.StartPromptyApiAsync();
                 // Espera breve para dar tiempo al servidor
                 await Task.Delay(4000);
             }
             catch(Exception ex)
             {
                 Mensajes.Add(new ChatMessage { Rol = "Error", Texto = $"Fallo al iniciar API: {ex.Message}" });
             }
        }

        // 2. Enviar al backend (PROMPTY Lite)
        // La versión Lite no maneja historial complejo ni acciones, solo texto.
        var respuestaTexto = await _promptyClient.AskAsync(texto);

        // 3. Mostrar respuesta
        Mensajes.Add(new ChatMessage
        {
            Rol = "PROMPTY",
            Texto = respuestaTexto
        });

        IsSending = false;
    }
}
