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

    public async Task<bool> HasValidTokenAsync(CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/config/token-status", ct);
            if (!response.IsSuccessStatusCode) return false;

            var body = await response.Content.ReadAsStringAsync(ct);
            using var doc = JsonDocument.Parse(body);
            if (doc.RootElement.TryGetProperty("has_token", out var prop))
            {
                return prop.GetBoolean();
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task<(bool Success, string ErrorMessage)> SetTokenWithDetailsAsync(string token, CancellationToken ct = default)
    {
        try
        {
            var payload = new { api_token = token };
            var json = JsonSerializer.Serialize(payload);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/config/set-token", content, ct);
            if (!response.IsSuccessStatusCode) 
                return (false, $"Error HTTP {response.StatusCode}");

            var body = await response.Content.ReadAsStringAsync(ct);
            using var doc = JsonDocument.Parse(body);
            
            bool ok = false;
            string errorDetails = "Token inválido";

            if (doc.RootElement.TryGetProperty("ok", out var prop))
            {
                ok = prop.GetBoolean();
            }

            if (!ok && doc.RootElement.TryGetProperty("details", out var detailsProp))
            {
                errorDetails = detailsProp.GetString() ?? errorDetails;
            }
            else if (!ok && doc.RootElement.TryGetProperty("error", out var errorProp))
            {
                errorDetails = errorProp.GetString() ?? errorDetails;
            }

            return (ok, ok ? "" : errorDetails);
        }
        catch (Exception ex)
        {
            return (false, $"Excepción: {ex.Message}");
        }
    }

    // Mantener compatibilidad si alguien usa la versión vieja, o redirigir
    public Task<bool> SetTokenAsync(string token, CancellationToken ct = default)
    {
        return Task.FromResult(false); // Deprecated in interface, removed from interface but kept here if needed by old code? 
        // Actually I removed it from interface, so I can remove it here or rename.
        // But wait, I removed SetTokenAsync from interface and added SetTokenWithDetailsAsync.
        // So I should remove SetTokenAsync from here.
    }
}
