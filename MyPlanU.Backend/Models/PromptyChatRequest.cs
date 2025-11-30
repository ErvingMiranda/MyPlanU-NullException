using System.Text.Json.Serialization;

namespace MyPlanU.Backend.Models;

public class PromptyChatRequest
{
    [JsonPropertyName("mensaje")]
    public string Mensaje { get; set; } = string.Empty;

    [JsonPropertyName("historial")]
    public List<PromptyHistoryItem>? Historial { get; set; }
}

public class PromptyHistoryItem
{
    [JsonPropertyName("rol")]
    public string Rol { get; set; } = string.Empty; // "usuario" o "assistant"

    [JsonPropertyName("contenido")]
    public string Contenido { get; set; } = string.Empty;
}
