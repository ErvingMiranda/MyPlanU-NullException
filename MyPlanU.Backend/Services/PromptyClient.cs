using System.Text;
using System.Text.Json;
using MyPlanU.Backend.Models;

namespace MyPlanU.Backend.Services;

public class PromptyClient : IPromptyClient
{
    private readonly HttpClient _httpClient;

    public PromptyClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PromptyChatResponse> EnviarMensajeAsync(
        string mensaje,
        List<PromptyHistoryItem>? historial = null,
        CancellationToken cancellationToken = default)
    {
        var request = new PromptyChatRequest
        {
            Mensaje = mensaje,
            Historial = historial
        };

        var json = JsonSerializer.Serialize(request);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/api/chat", content, cancellationToken);

        if ((int)response.StatusCode == 422)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            Console.WriteLine($"[PROMPTY] Error 422. Enviado: {json}");
            Console.WriteLine($"[PROMPTY] Recibido: {errorBody}");
            // Podríamos lanzar excepción o devolver un error formateado
            return new PromptyChatResponse { Respuesta = $"Error de validación (422): {errorBody}" };
        }

        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        var result = JsonSerializer.Deserialize<PromptyChatResponse>(body);

        return result ?? new PromptyChatResponse { Respuesta = "Error al interpretar la respuesta de PROMPTY." };
    }
}
