using MyPlanU.Models;

namespace MyPlanU.Data.Repositories;

public class RecordatorioRepository : IRecordatorioRepository
{
    private readonly AppDatabase _database;

    public RecordatorioRepository(AppDatabase database)
    {
        _database = database;
    }

    public Task<int> CreateAsync(Recordatorio recordatorio)
    {
        recordatorio.FechaCreacion = DateTime.UtcNow;
        return _database.Connection.InsertAsync(recordatorio);
    }

    public Task<int> UpdateAsync(Recordatorio recordatorio)
    {
        return _database.Connection.UpdateAsync(recordatorio);
    }

    public async Task<int> DeleteAsync(int idRecordatorio)
    {
        var rec = await GetByIdAsync(idRecordatorio);
        if (rec is null) return 0;
        return await _database.Connection.DeleteAsync(rec);
    }

    public Task<Recordatorio?> GetByIdAsync(int idRecordatorio) =>
        _database.Connection.Table<Recordatorio>()
            .Where(r => r.IdRecordatorio == idRecordatorio)
            .FirstOrDefaultAsync();

    public Task<List<Recordatorio>> GetActivosPendientesAsync(DateTime hasta) =>
        _database.Connection.Table<Recordatorio>()
            .Where(r => r.Activo && r.FechaHora <= hasta)
            .OrderBy(r => r.FechaHora)
            .ToListAsync();
}
