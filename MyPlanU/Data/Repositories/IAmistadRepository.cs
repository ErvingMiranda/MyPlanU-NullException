using MyPlanU.Models;

namespace MyPlanU.Data.Repositories;

public interface IAmistadRepository
{
    Task<int> CreateAsync(Amistad amistad);
    Task<int> UpdateAsync(Amistad amistad);
    Task<int> DeleteAsync(int idAmistad);

    Task<Amistad?> GetByIdAsync(int idAmistad);

    Task<List<Amistad>> GetAmistadesDeUsuarioAsync(int idUsuario);

    Task<Amistad?> GetRelacionAsync(int idUsuarioPrincipal, int idUsuarioAmigo);
}
