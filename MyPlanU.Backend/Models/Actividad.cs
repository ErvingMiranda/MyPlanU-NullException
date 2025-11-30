using SQLite;

namespace MyPlanU.Backend.Models;

public class Actividad
{
    [PrimaryKey, AutoIncrement]
    public int IdActividad { get; set; }
    [Indexed]
    public int IdUsuarioCreador { get; set; }
    public string Titulo { get; set; }
    public string Descripcion { get; set; }
    public string Prioridad { get; set; }
    public string Estado { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public bool TodoElDia { get; set; }
    public DateTime? FechaCompletado { get; set; }
    public string NotaRapida { get; set; }
    public int? IdRecordatorio { get; set; }
}
