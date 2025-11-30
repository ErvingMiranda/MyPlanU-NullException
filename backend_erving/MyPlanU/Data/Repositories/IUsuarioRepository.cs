using MyPlanU.Models;

namespace MyPlanU.Data.Repositories;

public interface IUsuarioRepository
{
    Task<Usuario?> GetByIdAsync(int idUsuario);
    Task<Usuario?> GetByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email);

    Task<Usuario?> GetByEmailAndPasswordHashAsync(string email, string contrasenaHash);

    Task<int> CreateAsync(Usuario usuario);
    Task<int> UpdateAsync(Usuario usuario);
}
