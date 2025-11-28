using System.Collections.ObjectModel;
using MyPlanU.Models;
using MyPlanU.Services;

namespace MyPlanU;

public partial class PromptyPage : ContentPage
{
    private readonly IPromptyClient _prompty;
    private bool _isSending;

    public ObservableCollection<ChatMessage> Mensajes { get; } = new();

    // INYECCIÓN POR CONSTRUCTOR: el contenedor nos pasa IPromptyClient
    public PromptyPage(IPromptyClient prompty)
    {
        InitializeComponent();

        _prompty = prompty ?? throw new ArgumentNullException(nameof(prompty));

        BindingContext = this;

        Mensajes.Add(new ChatMessage
        {
            Rol = "PROMPTY",
            Texto = "Hola, soy PROMPTY. ¿En qué te ayudo con tu planificación?"
        });
    }

    private async void OnSendClicked(object sender, EventArgs e)
    {
        await EnviarMensajeAsync();
    }

    private async void OnSendCompleted(object sender, EventArgs e)
    {
        await EnviarMensajeAsync();
    }

    private async Task EnviarMensajeAsync()
    {
        if (_isSending)
            return;

        var texto = MensajeEntry?.Text?.Trim();
        if (string.IsNullOrEmpty(texto))
            return;

        _isSending = true;
        SetUiEnabled(false);

        try
        {
            // Mensaje del usuario
            Mensajes.Add(new ChatMessage
            {
                Rol = "Usuario",
                Texto = texto
            });

            MensajeEntry.Text = string.Empty;

            // Historial simple para la API (opcional, pero future-proof)
            var historial = Mensajes.Select(m => new HistorialItem
            {
                Rol = m.Rol.Equals("PROMPTY", StringComparison.OrdinalIgnoreCase)
                    ? "asistente"
                    : "usuario",
                Contenido = m.Texto
            });

            var respuesta = await _prompty.EnviarMensajeAsync(texto, historial);

            string textoPrompty;
            if (respuesta == null)
            {
                textoPrompty = "No recibí respuesta de PROMPTY.";
            }
            else if (!respuesta.Exito)
            {
                textoPrompty = string.IsNullOrWhiteSpace(respuesta.Respuesta)
                    ? "Ocurrió un problema al procesar tu mensaje."
                    : respuesta.Respuesta;
            }
            else
            {
                textoPrompty = string.IsNullOrWhiteSpace(respuesta.Respuesta)
                    ? "(PROMPTY no devolvió texto.)"
                    : respuesta.Respuesta;
            }

            Mensajes.Add(new ChatMessage
            {
                Rol = "PROMPTY",
                Texto = textoPrompty
            });

            // Scroll al último mensaje por índice
            await Task.Yield(); // deja que la UI pinte el nuevo ítem
            if (Mensajes.Count > 0)
            {
                MensajesList?.ScrollTo(Mensajes.Count - 1,
                    position: ScrollToPosition.End,
                    animate: true);
            }
        }
        catch (Exception)
        {
            await DisplayAlert(
                "Error",
                "No fue posible contactar a PROMPTY en este momento. Asegúrate de que el servidor esté ejecutándose.",
                "OK");
        }
        finally
        {
            SetUiEnabled(true);
            _isSending = false;
        }
    }

    private void SetUiEnabled(bool enabled)
    {
        if (SendButton != null)
            SendButton.IsEnabled = enabled;

        if (MensajeEntry != null)
            MensajeEntry.IsEnabled = enabled;
    }
}
