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

        // 2. Preparar historial
        var historial = Mensajes.Select(m => new PromptyHistoryItem
        {
            Rol = m.Rol.Equals("PROMPTY", StringComparison.OrdinalIgnoreCase) ? "assistant" : "usuario",
            Contenido = m.Texto
        }).ToList();

        // 3. Enviar al backend
        var respuesta = await _promptyClient.EnviarMensajeAsync(texto, historial);

        // 4. Mostrar respuesta
        if (respuesta != null)
        {
            Mensajes.Add(new ChatMessage
            {
                Rol = "PROMPTY",
                Texto = respuesta.Respuesta
            });
        }
        else
        {
            Mensajes.Add(new ChatMessage
            {
                Rol = "PROMPTY",
                Texto = "Error desconocido al recibir respuesta."
            });
        }

        IsSending = false;
    }
}
