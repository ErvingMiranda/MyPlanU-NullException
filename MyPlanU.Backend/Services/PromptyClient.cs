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

    public async Task<PromptyChatResponse?> EnviarMensajeAsync(
        string mensaje,
        List<PromptyHistoryItem> historial)
    {
        try
        {
            var request = new PromptyChatRequest
            {
                Mensaje = mensaje,
                Historial = historial
            };

            var json = JsonSerializer.Serialize(request);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/chat", content);

            if (!response.IsSuccessStatusCode)
            {
                return new PromptyChatResponse
                {
                    Respuesta = $"Error al conectar con el servicio. Status: {response.StatusCode}",
                    Exito = false
                };
            }

            var body = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<PromptyChatResponse>(body);

            return result ?? new PromptyChatResponse { Respuesta = "Respuesta vacía del servidor.", Exito = false };
        }
        catch (Exception ex)
        {
            return new PromptyChatResponse
            {
                Respuesta = $"Excepción: {ex.Message}",
                Exito = false
            };
        }
    }
}
