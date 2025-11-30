using SQLite;

namespace MyPlanU.Models;

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

    // "lector", "colaborador"
    public string RolCompartido { get; set; } = "lector";

    // "pendiente", "aceptada", "rechazada"
    public string EstadoCompartir { get; set; } = "pendiente";

    public DateTime FechaCompartida { get; set; } = DateTime.UtcNow;
    public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;
}
