using SQLite;

namespace MyPlanU.Backend.Models;

public class PromptyConfig
{
    [PrimaryKey, AutoIncrement]
    public int IdPrompty { get; set; }
    [Unique]
    public int IdUsuario { get; set; }
    public string NombreAsistente { get; set; }
    
    [Obsolete("Configuración de voz no implementada.")]
    public string WakeWord { get; set; }
    
    public bool Activo { get; set; }
    
    [Obsolete("Configuración de voz no implementada.")]
    public string VozNombre { get; set; }
    
    [Obsolete("Configuración de voz no implementada.")]
    public float VelocidadVoz { get; set; }
    
    [Obsolete("Configuración de voz no implementada.")]
    public float VolumenVoz { get; set; }
    
    [Obsolete("Configuración de voz no implementada.")]
    public string TonoVoz { get; set; }
    
    public DateTime FechaCreacion { get; set; }
    public DateTime FechaActualizacion { get; set; }
}
