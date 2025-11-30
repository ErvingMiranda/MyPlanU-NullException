using MyPlanU.Models;

namespace MyPlanU.Data.Repositories;

public interface IActividadRepository
{
    Task<int> CreateAsync(Actividad actividad);
    Task<int> UpdateAsync(Actividad actividad);
    Task<int> DeleteAsync(int idActividad);

    Task<Actividad?> GetByIdAsync(int idActividad);

    Task<List<Actividad>> GetByUsuarioAsync(int idUsuario);

    Task<List<Actividad>> GetByUsuarioAndRangoFechaAsync(
        int idUsuario,
        DateTime fechaDesde,
        DateTime fechaHasta);
}
