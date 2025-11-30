using System.Text.Json.Serialization;

namespace MyPlanU.Backend.Models;

public class PromptyChatResponse
{
    [JsonPropertyName("respuesta")]
    public string Respuesta { get; set; } = string.Empty;

    [JsonPropertyName("exito")]
    public bool Exito { get; set; }
}
