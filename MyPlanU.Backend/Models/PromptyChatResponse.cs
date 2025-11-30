using System.Collections.Generic;

namespace MyPlanU.Backend.Models;

public class PromptyChatResponse
{
    public string Respuesta { get; set; } = string.Empty;
    public string? Accion { get; set; }   // "decir_hora", "abrir_youtube" o null
    public Dictionary<string, object>? Argumentos { get; set; }
}
