using MyPlanU.Models;

namespace MyPlanU.Data.Repositories;

public class ActividadRepository : IActividadRepository
{
    private readonly AppDatabase _database;

    public ActividadRepository(AppDatabase database)
    {
        _database = database;
    }

    public Task<int> CreateAsync(Actividad actividad)
    {
        actividad.FechaCreacion = DateTime.UtcNow;
        return _database.Connection.InsertAsync(actividad);
    }

    public Task<int> UpdateAsync(Actividad actividad)
    {
        if (actividad.Estado == "completada" && actividad.FechaCompletado == null)
        {
            actividad.FechaCompletado = DateTime.UtcNow;
        }

        return _database.Connection.UpdateAsync(actividad);
    }

    public async Task<int> DeleteAsync(int idActividad)
    {
        var act = await GetByIdAsync(idActividad);
        if (act is null) return 0;
        return await _database.Connection.DeleteAsync(act);
    }

    public Task<Actividad?> GetByIdAsync(int idActividad) =>
        _database.Connection.Table<Actividad>()
            .Where(a => a.IdActividad == idActividad)
            .FirstOrDefaultAsync();

    public Task<List<Actividad>> GetByUsuarioAsync(int idUsuario) =>
        _database.Connection.Table<Actividad>()
            .Where(a => a.IdUsuarioCreador == idUsuario)
            .OrderBy(a => a.FechaInicio ?? a.FechaFin ?? a.FechaCreacion)
            .ToListAsync();

    public Task<List<Actividad>> GetByUsuarioAndRangoFechaAsync(
        int idUsuario,
        DateTime fechaDesde,
        DateTime fechaHasta)
    {
        var desde = fechaDesde;
        var hasta = fechaHasta;

        return _database.Connection.Table<Actividad>()
            .Where(a =>
                a.IdUsuarioCreador == idUsuario &&
                ((a.FechaInicio ?? a.FechaCreacion) >= desde) &&
                ((a.FechaInicio ?? a.FechaCreacion) <= hasta))
            .OrderBy(a => a.FechaInicio ?? a.FechaCreacion)
            .ToListAsync();
    }
}
