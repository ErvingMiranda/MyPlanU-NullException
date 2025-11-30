using MyPlanU.Backend.Data.Repositories;
using MyPlanU.Backend.Models;

namespace MyPlanU.Backend.Business;

public class ActividadCompartidaService
{
    private readonly IActividadCompartidaRepository _repository;

    public ActividadCompartidaService(IActividadCompartidaRepository repository)
    {
        _repository = repository;
    }

    public Task<List<ActividadCompartida>> GetCompartidasAsync(int idUsuario)
    {
        return _repository.GetActividadesCompartidasAsync(idUsuario);
    }
}
