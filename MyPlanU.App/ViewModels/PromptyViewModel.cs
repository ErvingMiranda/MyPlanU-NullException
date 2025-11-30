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

        var historial = Mensajes.Select(m => new PromptyMensaje
        {
            Rol = m.Rol.Equals("PROMPTY", StringComparison.OrdinalIgnoreCase) ? "asistente" : "usuario",
            Contenido = m.Texto
        }).ToList();

        try
        {
            var respuesta = await _promptyClient.EnviarMensajeAsync(texto, historial);

            Mensajes.Add(new ChatMessage
            {
                Rol = "PROMPTY",
                Texto = respuesta.Respuesta
            });

            await EjecutarAccionPromptyAsync(respuesta);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"No se pudo contactar con PROMPTY: {ex.Message}", "OK");
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
        if (string.IsNullOrWhiteSpace(respuesta.Accion))
            return;

        switch (respuesta.Accion)
        {
            case "decir_hora":
                var ahora = DateTime.Now;
                var textoHora = $"La hora actual es: {ahora:HH:mm}";
                Mensajes.Add(new ChatMessage { Rol = "PROMPTY", Texto = textoHora });
                break;

            case "abrir_youtube":
                if (respuesta.Argumentos != null && respuesta.Argumentos.TryGetValue("query", out var qObj))
                {
                    string? query = null;
                    if (qObj is JsonElement qElement && qElement.ValueKind == JsonValueKind.String)
                        query = qElement.GetString();
                    else if (qObj is string qStr)
                        query = qStr;

                    if (string.IsNullOrWhiteSpace(query))
                        query = "youtube";

                    var url = $"https://www.youtube.com/results?search_query={Uri.EscapeDataString(query)}";
                    await Browser.Default.OpenAsync(url, BrowserLaunchMode.SystemPreferred);
                }
                break;

            default:
                // Acción desconocida: por ahora no hacer nada extra.
                break;
        }
    }
}
