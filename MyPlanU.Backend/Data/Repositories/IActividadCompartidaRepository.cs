using MyPlanU.Backend.Models;

namespace MyPlanU.Backend.Data.Repositories;

public interface IActividadCompartidaRepository
{
    Task<List<ActividadCompartida>> GetActividadesCompartidasAsync(int idUsuario);
    Task<int> SaveActividadCompartidaAsync(ActividadCompartida actividadCompartida);
    Task<int> DeleteActividadCompartidaAsync(ActividadCompartida actividadCompartida);
}
