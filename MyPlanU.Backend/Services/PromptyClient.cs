using System.Text;
using System.Text.Json;
using MyPlanU.Backend.Models;

namespace MyPlanU.Backend.Services;

public class PromptyClient : IPromptyClient
{
    private readonly HttpClient _http;

    public PromptyClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<PromptyChatResponse> EnviarMensajeAsync(string mensaje, IEnumerable<HistorialItem>? historial = null)
    {
        var payload = new
        {
            mensaje = mensaje,
            historial = (historial ?? new List<HistorialItem>()).Select(h => new { rol = h.Rol, contenido = h.Contenido }).ToList()
        };

        try 
        {
            var json = JsonSerializer.Serialize(payload);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _http.PostAsync("/api/chat", content);
            
            if (!response.IsSuccessStatusCode)
            {
                return new PromptyChatResponse
                {
                    Exito = false,
                    Respuesta = $"Error HTTP {response.StatusCode}"
                };
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<PromptyChatResponse>(jsonResponse, opciones);
            
            return result ?? new PromptyChatResponse { Exito = false, Respuesta = "Error al procesar la respuesta de PROMPTY." };
        }
        catch (Exception ex)
        {
             return new PromptyChatResponse
            {
                Exito = false,
                Respuesta = $"Error de conexión: {ex.Message}"
            };
        }
    }
}
