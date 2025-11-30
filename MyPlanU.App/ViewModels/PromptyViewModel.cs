using System.Collections.ObjectModel;
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

        var respuesta = await _promptyClient.EnviarMensajeAsync(texto, historial);

        Mensajes.Add(new ChatMessage
        {
            Rol = "PROMPTY",
            Texto = respuesta.Exito ? respuesta.Respuesta : $"Error: {respuesta.Respuesta}"
        });

        IsSending = false;
    }
}
