using SQLite;

namespace MyPlanU.Backend.Models;

public class Amistad
{
    [PrimaryKey, AutoIncrement]
    public int IdAmistad { get; set; }
    [Indexed]
    public int IdUsuarioPrincipal { get; set; }
    [Indexed]
    public int IdUsuarioAmigo { get; set; }
    
    [Obsolete("Propiedad definida en el modelo pero sin referencias de uso en el código.")]
    public string? Alias { get; set; }
    
    public string? EstadoSolicitud { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime FechaActualizacion { get; set; }
}
