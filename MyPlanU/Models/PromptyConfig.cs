using SQLite;

namespace MyPlanU.Models;

public class PromptyConfig
{
    [PrimaryKey, AutoIncrement]
    public int IdPrompty { get; set; }

    [Indexed(Unique = true)]
    public int IdUsuario { get; set; }

    public string NombreAsistente { get; set; } = "PROMPTY";

    public string WakeWord { get; set; } = "prompty";

    public bool Activo { get; set; } = true;

    public string VozNombre { get; set; } = "default";

    public double VelocidadVoz { get; set; } = 1.0;

    public double VolumenVoz { get; set; } = 1.0;

    public double TonoVoz { get; set; } = 1.0;

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;
}
