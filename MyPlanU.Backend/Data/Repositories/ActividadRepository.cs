using MyPlanU.Backend.Models;

namespace MyPlanU.Backend.Data.Repositories;

public class ActividadRepository : IActividadRepository
{
    private readonly SQLiteContext _context;

    public ActividadRepository(SQLiteContext context)
    {
        _context = context;
    }

    private async Task InitAsync() => await _context.InitAsync();

    public async Task<List<Actividad>> GetActividadesAsync()
    {
        await InitAsync();
        return await _context.Connection.Table<Actividad>().ToListAsync();
    }

    public async Task<List<Actividad>> GetActividadesByUsuarioAsync(int idUsuario)
    {
        await InitAsync();
        return await _context.Connection.Table<Actividad>().Where(x => x.IdUsuarioCreador == idUsuario).ToListAsync();
    }

    public async Task<Actividad> GetActividadAsync(int id)
    {
        await InitAsync();
        return await _context.Connection.Table<Actividad>().Where(i => i.IdActividad == id).FirstOrDefaultAsync();
    }

    public async Task<int> SaveActividadAsync(Actividad actividad)
    {
        await InitAsync();
        if (actividad.IdActividad != 0)
            return await _context.Connection.UpdateAsync(actividad);
        else
            return await _context.Connection.InsertAsync(actividad);
    }

    public async Task<int> DeleteActividadAsync(Actividad actividad)
    {
        await InitAsync();
        return await _context.Connection.DeleteAsync(actividad);
    }
}
