using MyPlanU.Backend.Models;

namespace MyPlanU.Backend.Data.Repositories;

public class RecordatorioRepository : IRecordatorioRepository
{
    private readonly SQLiteContext _context;

    public RecordatorioRepository(SQLiteContext context)
    {
        _context = context;
    }

    private async Task InitAsync() => await _context.InitAsync();

    public async Task<Recordatorio> GetRecordatorioAsync(int id)
    {
        await InitAsync();
        return await _context.Connection.Table<Recordatorio>().Where(i => i.IdRecordatorio == id).FirstOrDefaultAsync();
    }

    public async Task<int> SaveRecordatorioAsync(Recordatorio recordatorio)
    {
        await InitAsync();
        if (recordatorio.IdRecordatorio != 0)
            return await _context.Connection.UpdateAsync(recordatorio);
        else
            return await _context.Connection.InsertAsync(recordatorio);
    }

    public async Task<int> DeleteRecordatorioAsync(Recordatorio recordatorio)
    {
        await InitAsync();
        return await _context.Connection.DeleteAsync(recordatorio);
    }
}
