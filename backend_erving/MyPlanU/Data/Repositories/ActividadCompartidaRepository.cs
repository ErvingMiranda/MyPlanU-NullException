using MyPlanU.Models;

namespace MyPlanU.Data.Repositories;

public class ActividadCompartidaRepository : IActividadCompartidaRepository
{
    private readonly AppDatabase _database;

    public ActividadCompartidaRepository(AppDatabase database)
    {
        _database = database;
    }

    public Task<int> CreateAsync(ActividadCompartida compartir)
    {
        compartir.FechaCompartida = DateTime.UtcNow;
        compartir.FechaActualizacion = DateTime.UtcNow;
        return _database.Connection.InsertAsync(compartir);
    }

    public Task<int> UpdateAsync(ActividadCompartida compartir)
    {
        compartir.FechaActualizacion = DateTime.UtcNow;
        return _database.Connection.UpdateAsync(compartir);
    }

    public async Task<int> DeleteAsync(int idCompartirActividad)
    {
        var c = await GetByIdAsync(idCompartirActividad);
        if (c is null) return 0;
        return await _database.Connection.DeleteAsync(c);
    }

    public Task<ActividadCompartida?> GetByIdAsync(int idCompartirActividad) =>
        _database.Connection.Table<ActividadCompartida>()
            .Where(c => c.IdCompartirActividad == idCompartirActividad)
            .FirstOrDefaultAsync();

    public Task<List<ActividadCompartida>> GetCompartidasPorUsuarioAsync(int idUsuarioDestino) =>
        _database.Connection.Table<ActividadCompartida>()
            .Where(c => c.IdUsuarioDestino == idUsuarioDestino)
            .ToListAsync();

    public Task<List<ActividadCompartida>> GetCompartidasDePropietarioAsync(int idUsuarioPropietario) =>
        _database.Connection.Table<ActividadCompartida>()
            .Where(c => c.IdUsuarioPropietario == idUsuarioPropietario)
            .ToListAsync();
}
