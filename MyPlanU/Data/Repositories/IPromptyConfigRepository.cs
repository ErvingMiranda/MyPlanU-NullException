using MyPlanU.Models;

namespace MyPlanU.Data.Repositories;

public interface IPromptyConfigRepository
{
    Task<PromptyConfig?> GetByUsuarioAsync(int idUsuario);
    Task<int> CreateAsync(PromptyConfig config);
    Task<int> UpdateAsync(PromptyConfig config);

    Task<int> SaveForUsuarioAsync(PromptyConfig config);
}
