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
    
    [ObservableProperty]
    private ObservableCollection<ChatMessage> mensajes = new();

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(EnviarMensajeCommand))]
    private string mensajeEntrada = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(EnviarMensajeCommand))]
    private bool isSending;

    public PromptyViewModel(IPromptyLiteClient promptyClient)
    {
        _promptyClient = promptyClient;
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
