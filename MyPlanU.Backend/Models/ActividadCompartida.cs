using SQLite;

namespace MyPlanU.Backend.Models;

public class ActividadCompartida
{
    [PrimaryKey, AutoIncrement]
    public int IdCompartirActividad { get; set; }
    [Indexed]
    public int IdActividad { get; set; }
    [Indexed]
    public int IdUsuarioPropietario { get; set; }
    [Indexed]
    public int IdUsuarioDestino { get; set; }
    public string RolCompartido { get; set; }
    public string EstadoCompartir { get; set; }
    public DateTime FechaCompartida { get; set; }
    public DateTime FechaActualizacion { get; set; }
}
