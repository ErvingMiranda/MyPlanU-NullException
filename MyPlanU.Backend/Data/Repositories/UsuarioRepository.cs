using MyPlanU.Backend.Models;

namespace MyPlanU.Backend.Data.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly SQLiteContext _context;

    public UsuarioRepository(SQLiteContext context)
    {
        _context = context;
    }

    private async Task InitAsync() => await _context.InitAsync();

    public async Task<Usuario> GetUsuarioAsync(int id)
    {
        await InitAsync();
        return await _context.Connection.Table<Usuario>().Where(i => i.IdUsuario == id).FirstOrDefaultAsync();
    }

    public async Task<Usuario> GetUsuarioByEmailAsync(string email)
    {
        await InitAsync();
        return await _context.Connection.Table<Usuario>().Where(i => i.Email == email).FirstOrDefaultAsync();
    }

    public async Task<int> SaveUsuarioAsync(Usuario usuario)
    {
        await InitAsync();
        if (usuario.IdUsuario != 0)
            return await _context.Connection.UpdateAsync(usuario);
        else
            return await _context.Connection.InsertAsync(usuario);
    }

    public async Task<int> DeleteUsuarioAsync(Usuario usuario)
    {
        await InitAsync();
        return await _context.Connection.DeleteAsync(usuario);
    }
}
