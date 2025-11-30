using MyPlanU.Backend.Models;

namespace MyPlanU.Backend.Services;

public interface IPromptyClient
{
    Task<PromptyChatResponse> EnviarMensajeAsync(string mensaje, IEnumerable<HistorialItem>? historial = null);
}

public class HistorialItem
{
    public string Rol { get; set; } = "";
    public string Contenido { get; set; } = "";
}
