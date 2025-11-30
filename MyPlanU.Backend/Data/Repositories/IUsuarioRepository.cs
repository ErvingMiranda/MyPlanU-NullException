using MyPlanU.Backend.Models;

namespace MyPlanU.Backend.Data.Repositories;

public interface IUsuarioRepository
{
    Task<Usuario> GetUsuarioAsync(int id);
    Task<Usuario> GetUsuarioByEmailAsync(string email);
    Task<int> SaveUsuarioAsync(Usuario usuario);
    Task<int> DeleteUsuarioAsync(Usuario usuario);
}
