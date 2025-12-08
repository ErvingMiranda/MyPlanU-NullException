using MyPlanU.Backend.Data;
using MyPlanU.Backend.Data.Repositories;
using MyPlanU.Backend.Models;
using SQLite;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyPlanU.Backend.Business;

public class ActividadCompartidaService
{
    private readonly SQLiteContext _context;
    private readonly IActividadRepository _actividadRepository;

    public ActividadCompartidaService(SQLiteContext context, IActividadRepository actividadRepository)
    {
        _context = context;
        _actividadRepository = actividadRepository;
    }

    private async Task InitAsync() => await _context.InitAsync();

    public async Task<bool> CompartirActividadAsync(int idActividad, int idUsuarioPropietario, int idUsuarioDestino, string rolCompartido)
    {
        await InitAsync();

        // 1. Validar que el propietario sea el dueño de la actividad
        var actividad = await _actividadRepository.GetActividadAsync(idActividad);
        if (actividad == null || actividad.IdUsuario != idUsuarioPropietario)
        {
            return false;
        }

        // 2. Evitar duplicados (misma actividad, mismo destino, estado Activo)
        var existing = await _context.Connection.Table<ActividadCompartida>()
            .Where(ac => ac.IdActividad == idActividad && 
                         ac.IdUsuarioDestino == idUsuarioDestino && 
                         ac.EstadoCompartir == "Activo")
            .FirstOrDefaultAsync();

        if (existing != null)
        {
            return false; // Ya está compartida
        }

        // 3. Crear registro
        var nuevaCompartida = new ActividadCompartida
        {
            IdActividad = idActividad,
            IdUsuarioPropietario = idUsuarioPropietario,
            IdUsuarioDestino = idUsuarioDestino,
            RolCompartido = rolCompartido,
            EstadoCompartir = "Activo",
            FechaCompartida = DateTime.UtcNow,
            FechaActualizacion = DateTime.UtcNow
        };

        await _context.Connection.InsertAsync(nuevaCompartida);
        return true;
    }

    public async Task<List<Actividad>> ObtenerActividadesCompartidasConmigoAsync(int idUsuarioDestino)
    {
        await InitAsync();

        // Obtener IDs de actividades compartidas conmigo
        var compartidas = await _context.Connection.Table<ActividadCompartida>()
            .Where(ac => ac.IdUsuarioDestino == idUsuarioDestino && ac.EstadoCompartir == "Activo")
            .ToListAsync();

        var actividades = new List<Actividad>();
        foreach (var c in compartidas)
        {
            var act = await _actividadRepository.GetActividadAsync(c.IdActividad);
            if (act != null)
            {
                actividades.Add(act);
            }
        }
        return actividades;
    }

    public async Task<List<Actividad>> ObtenerActividadesQueCompartoAsync(int idUsuarioPropietario)
    {
        await InitAsync();

        // Obtener IDs de actividades que yo comparto
        var compartidas = await _context.Connection.Table<ActividadCompartida>()
            .Where(ac => ac.IdUsuarioPropietario == idUsuarioPropietario && ac.EstadoCompartir == "Activo")
            .ToListAsync();

        // Podría haber duplicados si comparto la misma actividad con varias personas, 
        // así que usamos un HashSet o Distinct para devolver actividades únicas.
        var actividadIds = compartidas.Select(c => c.IdActividad).Distinct();

        var actividades = new List<Actividad>();
        foreach (var id in actividadIds)
        {
            var act = await _actividadRepository.GetActividadAsync(id);
            if (act != null)
            {
                actividades.Add(act);
            }
        }
        return actividades;
    }
}
