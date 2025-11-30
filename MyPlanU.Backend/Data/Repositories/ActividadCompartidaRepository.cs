using MyPlanU.Backend.Models;

namespace MyPlanU.Backend.Data.Repositories;

public class ActividadCompartidaRepository : IActividadCompartidaRepository
{
    private readonly SQLiteContext _context;

    public ActividadCompartidaRepository(SQLiteContext context)
    {
        _context = context;
    }

    private async Task InitAsync() => await _context.InitAsync();

    public async Task<List<ActividadCompartida>> GetActividadesCompartidasAsync(int idUsuario)
    {
        await InitAsync();
        return await _context.Connection.Table<ActividadCompartida>()
            .Where(x => x.IdUsuarioPropietario == idUsuario || x.IdUsuarioDestino == idUsuario)
            .ToListAsync();
    }

    public async Task<int> SaveActividadCompartidaAsync(ActividadCompartida actividadCompartida)
    {
        await InitAsync();
        if (actividadCompartida.IdCompartirActividad != 0)
            return await _context.Connection.UpdateAsync(actividadCompartida);
        else
            return await _context.Connection.InsertAsync(actividadCompartida);
    }

    public async Task<int> DeleteActividadCompartidaAsync(ActividadCompartida actividadCompartida)
    {
        await InitAsync();
        return await _context.Connection.DeleteAsync(actividadCompartida);
    }
}
