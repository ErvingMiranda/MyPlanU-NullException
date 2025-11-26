using SQLite;

namespace MyPlanU.Models;

public class Actividad
{
    [PrimaryKey, AutoIncrement]
    public int IdActividad { get; set; }

    [Indexed]
    public int IdUsuarioCreador { get; set; }

    [NotNull]
    public string Titulo { get; set; } = "";

    public string Descripcion { get; set; } = "";

    // "alta", "media", "baja"
    public string Prioridad { get; set; } = "media";

    // "pendiente", "en_progreso", "completada", "cancelada"
    public string Estado { get; set; } = "pendiente";

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }

    public bool TodoElDia { get; set; } = false;

    public DateTime? FechaCompletado { get; set; }

    public string NotaRapida { get; set; } = "";

    // opcional (0..1)
    [Indexed]
    public int? IdRecordatorio { get; set; }
}
