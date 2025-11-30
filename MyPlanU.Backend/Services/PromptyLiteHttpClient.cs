using System.Text;
using System.Text.Json;
using MyPlanU.Backend.Models;

namespace MyPlanU.Backend.Services;

public class PromptyLiteHttpClient : IPromptyLiteClient
{
    private readonly HttpClient _httpClient;

    public PromptyLiteHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> AskAsync(string userMessage, CancellationToken ct = default)
    {
        try
        {
            // Construimos el request. 
            // Nota: La interfaz Lite simplificada no pasa historial, 
            // así que enviamos una lista vacía o solo el mensaje actual si el backend lo soporta.
            // Asumimos que el backend espera la estructura PromptyChatRequest.
            var request = new PromptyChatRequest
            {
                Mensaje = userMessage,
                Historial = new List<PromptyHistoryItem>() // Sin historial en versión Lite por contrato simplificado
            };

            var json = JsonSerializer.Serialize(request);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/chat", content, ct);

            if (!response.IsSuccessStatusCode)
            {
                return $"Error: El servicio respondió con estado {response.StatusCode}";
            }

            var body = await response.Content.ReadAsStringAsync(ct);
            
            // Intentamos deserializar la respuesta esperada
            var result = JsonSerializer.Deserialize<PromptyChatResponse>(body);

            // Devolvemos SOLO el texto de respuesta
            return result?.Respuesta ?? "El asistente no devolvió ninguna respuesta.";
        }
        catch (Exception ex)
        {
            return $"Error de conexión: {ex.Message}";
        }
    }

    public async Task<bool> CheckHealthAsync(CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("/health", ct);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
