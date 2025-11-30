using MyPlanU.Models;

namespace MyPlanU.Data.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDatabase _database;

    public UsuarioRepository(AppDatabase database)
    {
        _database = database;
    }

    public Task<Usuario?> GetByIdAsync(int idUsuario) =>
        _database.Connection.Table<Usuario>()
            .Where(u => u.IdUsuario == idUsuario)
            .FirstOrDefaultAsync();

    public Task<Usuario?> GetByEmailAsync(string email) =>
        _database.Connection.Table<Usuario>()
            .Where(u => u.Email == email)
            .FirstOrDefaultAsync();

    public async Task<bool> EmailExistsAsync(string email)
    {
        var user = await GetByEmailAsync(email);
        return user != null;
    }

    public Task<Usuario?> GetByEmailAndPasswordHashAsync(string email, string contrasenaHash) =>
        _database.Connection.Table<Usuario>()
            .Where(u => u.Email == email && u.ContrasenaHash == contrasenaHash)
            .FirstOrDefaultAsync();

    public Task<int> CreateAsync(Usuario usuario)
    {
        usuario.FechaRegistro = DateTime.UtcNow;
        return _database.Connection.InsertAsync(usuario);
    }

    public Task<int> UpdateAsync(Usuario usuario)
    {
        return _database.Connection.UpdateAsync(usuario);
    }
}
