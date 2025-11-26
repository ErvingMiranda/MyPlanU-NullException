using SQLite;

namespace MyPlanU.Models;

public class Recordatorio
{
    [PrimaryKey, AutoIncrement]
    public int IdRecordatorio { get; set; }

    public string Titulo { get; set; } = "";

    public string Mensaje { get; set; } = "";

    [NotNull]
    public DateTime FechaHora { get; set; }

    // "app", "email", "push"
    public string Canal { get; set; } = "app";

    public bool Activo { get; set; } = true;

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}
