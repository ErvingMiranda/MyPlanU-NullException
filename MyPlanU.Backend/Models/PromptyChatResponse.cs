using System.Collections.Generic;

namespace MyPlanU.Backend.Models;

public class PromptyChatResponse
{
    public bool Exito { get; set; }
    public string Respuesta { get; set; } = string.Empty;
    public string? Accion { get; set; }
    public Dictionary<string, object>? Parametros { get; set; }
    public string Origen { get; set; } = "interprete";
}
