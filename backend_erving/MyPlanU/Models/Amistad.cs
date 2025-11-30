using SQLite;

namespace MyPlanU.Models;

public class Amistad
{
    [PrimaryKey, AutoIncrement]
    public int IdAmistad { get; set; }

    [Indexed]
    public int IdUsuarioPrincipal { get; set; }

    [Indexed]
    public int IdUsuarioAmigo { get; set; }

    public string Alias { get; set; } = "";

    // "pendiente", "aceptada", "rechazada", "bloqueada"
    public string EstadoSolicitud { get; set; } = "pendiente";

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;
}
