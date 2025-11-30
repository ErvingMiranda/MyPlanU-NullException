namespace MyPlanU.Services;

public interface IPromptyClient
{
    Task<PromptyResponse> EnviarMensajeAsync(
        string mensaje,
        IEnumerable<HistorialItem>? historial = null);
}

public class HistorialItem
{
    public string Rol { get; set; } = "";
    public string Contenido { get; set; } = "";
}

public class PromptyResponse
{
    public string Respuesta { get; set; } = "";
    public bool Exito { get; set; }
}
