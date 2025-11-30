using SQLite;

namespace MyPlanU.Backend.Models;

public class PromptyConfig
{
    [PrimaryKey, AutoIncrement]
    public int IdPrompty { get; set; }
    [Unique]
    public int IdUsuario { get; set; }
    public string NombreAsistente { get; set; }
    public string WakeWord { get; set; }
    public bool Activo { get; set; }
    public string VozNombre { get; set; }
    public float VelocidadVoz { get; set; }
    public float VolumenVoz { get; set; }
    public string TonoVoz { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime FechaActualizacion { get; set; }
}
