using MyPlanU.Models;

namespace MyPlanU.Data.Repositories;

public class PromptyConfigRepository : IPromptyConfigRepository
{
    private readonly AppDatabase _database;

    public PromptyConfigRepository(AppDatabase database)
    {
        _database = database;
    }

    public Task<PromptyConfig?> GetByUsuarioAsync(int idUsuario) =>
        _database.Connection.Table<PromptyConfig>()
            .Where(p => p.IdUsuario == idUsuario)
            .FirstOrDefaultAsync();

    public Task<int> CreateAsync(PromptyConfig config)
    {
        config.FechaCreacion = DateTime.UtcNow;
        config.FechaActualizacion = DateTime.UtcNow;
        return _database.Connection.InsertAsync(config);
    }

    public Task<int> UpdateAsync(PromptyConfig config)
    {
        config.FechaActualizacion = DateTime.UtcNow;
        return _database.Connection.UpdateAsync(config);
    }

    public async Task<int> SaveForUsuarioAsync(PromptyConfig config)
    {
        var existing = await GetByUsuarioAsync(config.IdUsuario);
        if (existing is null)
        {
            return await CreateAsync(config);
        }

        config.IdPrompty = existing.IdPrompty;
        return await UpdateAsync(config);
    }
}
