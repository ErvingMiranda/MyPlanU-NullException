using MyPlanU.Models;

namespace MyPlanU.Data.Repositories;

public class AmistadRepository : IAmistadRepository
{
    private readonly AppDatabase _database;

    public AmistadRepository(AppDatabase database)
    {
        _database = database;
    }

    public Task<int> CreateAsync(Amistad amistad)
    {
        amistad.FechaCreacion = DateTime.UtcNow;
        amistad.FechaActualizacion = DateTime.UtcNow;
        return _database.Connection.InsertAsync(amistad);
    }

    public Task<int> UpdateAsync(Amistad amistad)
    {
        amistad.FechaActualizacion = DateTime.UtcNow;
        return _database.Connection.UpdateAsync(amistad);
    }

    public async Task<int> DeleteAsync(int idAmistad)
    {
        var a = await GetByIdAsync(idAmistad);
        if (a is null) return 0;
        return await _database.Connection.DeleteAsync(a);
    }

    public Task<Amistad?> GetByIdAsync(int idAmistad) =>
        _database.Connection.Table<Amistad>()
            .Where(a => a.IdAmistad == idAmistad)
            .FirstOrDefaultAsync();

    public Task<List<Amistad>> GetAmistadesDeUsuarioAsync(int idUsuario) =>
        _database.Connection.Table<Amistad>()
            .Where(a => a.IdUsuarioPrincipal == idUsuario || a.IdUsuarioAmigo == idUsuario)
            .ToListAsync();

    public Task<Amistad?> GetRelacionAsync(int idUsuarioPrincipal, int idUsuarioAmigo) =>
        _database.Connection.Table<Amistad>()
            .Where(a =>
                a.IdUsuarioPrincipal == idUsuarioPrincipal &&
                a.IdUsuarioAmigo == idUsuarioAmigo)
            .FirstOrDefaultAsync();
}
