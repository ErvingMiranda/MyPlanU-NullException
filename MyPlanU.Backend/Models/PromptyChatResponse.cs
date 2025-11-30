using System.Text.Json.Serialization;

namespace MyPlanU.Backend.Models;

public class PromptyChatResponse
{
    [JsonPropertyName("respuesta")]
    public string Respuesta { get; set; } = string.Empty;

    [JsonPropertyName("accion")]
    public string? Accion { get; set; }

    [JsonPropertyName("argumentos")]
    public Dictionary<string, object>? Argumentos { get; set; }
}
