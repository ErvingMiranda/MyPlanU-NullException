using System.Collections.ObjectModel;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyPlanU.Backend.Models;
using MyPlanU.Backend.Services;

namespace MyPlanU.App.ViewModels;

public partial class PromptyViewModel : ObservableObject
{
    private readonly IPromptyClient _promptyClient;
    
    [ObservableProperty]
    private ObservableCollection<ChatMessage> mensajes = new();

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(EnviarMensajeCommand))]
    private string mensajeEntrada = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(EnviarMensajeCommand))]
    private bool isSending;

    public PromptyViewModel(IPromptyClient promptyClient)
    {
        _promptyClient = promptyClient;
        Mensajes.Add(new ChatMessage { Rol = "PROMPTY", Texto = "Hola, soy PROMPTY. ¿En qué te ayudo con tu planificación?" });
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

        Mensajes.Add(new ChatMessage { Rol = "Usuario", Texto = texto });

        var historial = Mensajes.Select(m => new HistorialItem
        {
            Rol = m.Rol.Equals("PROMPTY", StringComparison.OrdinalIgnoreCase) ? "asistente" : "usuario",
            Contenido = m.Texto
        });

        try
        {
            var respuesta = await _promptyClient.EnviarMensajeAsync(texto, historial);

            if (!respuesta.Exito)
            {
                await Shell.Current.DisplayAlert("Error", "PROMPTY no está disponible.", "OK");
            }

            Mensajes.Add(new ChatMessage
            {
                Rol = "PROMPTY",
                Texto = respuesta.Respuesta
            });

            await EjecutarAccionPromptyAsync(respuesta);
        }
        catch (Exception)
        {
            await Shell.Current.DisplayAlert("Error", "PROMPTY no está disponible.", "OK");
            Mensajes.Add(new ChatMessage
            {
                Rol = "PROMPTY",
                Texto = "Error al conectar con el servicio."
            });
        }
        finally
        {
            IsSending = false;
        }
    }

    private async Task EjecutarAccionPromptyAsync(PromptyChatResponse respuesta)
    {
        if (!respuesta.Exito || string.IsNullOrWhiteSpace(respuesta.Accion))
            return;

        switch (respuesta.Accion)
        {
            case "decir_hora":
                // Ya tienes DateTime.Now del sistema, úsalo para algo si quieres.
                // Por ahora, la hora ya viene en respuesta.Respuesta, así que no necesitas más.
                break;

            case "buscar_youtube":
                if (respuesta.Parametros != null && respuesta.Parametros.TryGetValue("query", out var qObj))
                {
                    string? query = null;
                    if (qObj is JsonElement qElement && qElement.ValueKind == JsonValueKind.String)
                        query = qElement.GetString();
                    else if (qObj is string qStr)
                        query = qStr;

                    if (!string.IsNullOrWhiteSpace(query))
                    {
                        var url = $"https://www.youtube.com/results?search_query={Uri.EscapeDataString(query)}";
                        await Browser.Default.OpenAsync(url, BrowserLaunchMode.External);
                    }
                }
                break;

            case "abrir_url":
                if (respuesta.Parametros != null && respuesta.Parametros.TryGetValue("url", out var urlObj))
                {
                    string? url = null;
                    if (urlObj is JsonElement urlElement && urlElement.ValueKind == JsonValueKind.String)
                        url = urlElement.GetString();
                    else if (urlObj is string urlStr)
                        url = urlStr;

                    if (url != null && Uri.TryCreate(url, UriKind.Absolute, out var uri))
                    {
                        await Browser.Default.OpenAsync(uri, BrowserLaunchMode.External);
                    }
                }
                break;

            default:
                // Acción desconocida: por ahora no hacer nada extra.
                break;
        }
    }
}
