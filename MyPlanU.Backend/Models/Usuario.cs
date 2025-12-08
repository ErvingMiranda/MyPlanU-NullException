using SQLite;

namespace MyPlanU.Backend.Models;

public class Usuario
{
    [PrimaryKey, AutoIncrement]
    public int IdUsuario { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string Apodo { get; set; }
    [Unique]
    public string Email { get; set; }
    public string ContrasenaHash { get; set; }
    public string AvatarUrl { get; set; }
    public string Pais { get; set; }
    public string ZonaHoraria { get; set; }
    public DateTime FechaRegistro { get; set; }
    public string EstadoCuenta { get; set; }
    public string PreguntaSeguridad { get; set; }
    public string RespuestaSeguridad { get; set; }
}
