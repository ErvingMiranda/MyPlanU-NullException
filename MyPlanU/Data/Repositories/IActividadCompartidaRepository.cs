using MyPlanU.Models;

namespace MyPlanU.Data.Repositories;

public interface IActividadCompartidaRepository
{
    Task<int> CreateAsync(ActividadCompartida compartir);
    Task<int> UpdateAsync(ActividadCompartida compartir);
    Task<int> DeleteAsync(int idCompartirActividad);

    Task<ActividadCompartida?> GetByIdAsync(int idCompartirActividad);

    Task<List<ActividadCompartida>> GetCompartidasPorUsuarioAsync(int idUsuarioDestino);

    Task<List<ActividadCompartida>> GetCompartidasDePropietarioAsync(int idUsuarioPropietario);
}
