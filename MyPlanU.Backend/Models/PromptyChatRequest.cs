using System.Collections.Generic;

namespace MyPlanU.Backend.Models;

public class PromptyChatRequest
{
    public string Mensaje { get; set; } = string.Empty;
    public List<PromptyMensaje>? Historial { get; set; }
}

public class PromptyMensaje
{
    public string Rol { get; set; } = string.Empty;       // "usuario" o "asistente"
    public string Contenido { get; set; } = string.Empty;
}
