using SQLite;

namespace MyPlanU.Models;

public class Usuario
{
    [PrimaryKey, AutoIncrement]
    public int IdUsuario { get; set; }

    [NotNull]
    public string Nombre { get; set; } = "";

    [NotNull]
    public string Apellido { get; set; } = "";

    public string Apodo { get; set; } = "";

    [NotNull]
    [Indexed(Unique = true)]
    public string Email { get; set; } = "";

    [NotNull]
    public string ContrasenaHash { get; set; } = "";

    public string AvatarUrl { get; set; } = "";

    public string Pais { get; set; } = "";

    public string ZonaHoraria { get; set; } = "";

    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    // "activa", "suspendida", "eliminada"
    public string EstadoCuenta { get; set; } = "activa";
}
