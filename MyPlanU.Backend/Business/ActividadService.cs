using MyPlanU.Backend.Data.Repositories;
using MyPlanU.Backend.Models;

namespace MyPlanU.Backend.Business;

public class ActividadService
{
    private readonly IActividadRepository _actividadRepository;

    public ActividadService(IActividadRepository actividadRepository)
    {
        _actividadRepository = actividadRepository;
    }

    public Task<List<Actividad>> GetActividadesPorUsuarioAsync(int idUsuario)
    {
        return _actividadRepository.GetActividadesByUsuarioAsync(idUsuario);
    }

    public Task SaveActividadAsync(Actividad actividad)
    {
        return _actividadRepository.SaveActividadAsync(actividad);
    }

    public Task DeleteActividadAsync(Actividad actividad)
    {
        return _actividadRepository.DeleteActividadAsync(actividad);
    }
}
