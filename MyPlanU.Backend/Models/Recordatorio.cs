using SQLite;

namespace MyPlanU.Backend.Models;

public class Recordatorio
{
    [PrimaryKey, AutoIncrement]
    public int IdRecordatorio { get; set; }
    public string Titulo { get; set; }
    public string Mensaje { get; set; }
    public DateTime FechaHora { get; set; }
    public string Canal { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaCreacion { get; set; }
}
