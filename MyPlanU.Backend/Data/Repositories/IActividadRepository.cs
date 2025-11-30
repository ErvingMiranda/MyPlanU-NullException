using MyPlanU.Backend.Models;

namespace MyPlanU.Backend.Data.Repositories;

public interface IActividadRepository
{
    Task<List<Actividad>> GetActividadesAsync();
    Task<List<Actividad>> GetActividadesByUsuarioAsync(int idUsuario);
    Task<Actividad> GetActividadAsync(int id);
    Task<int> SaveActividadAsync(Actividad actividad);
    Task<int> DeleteActividadAsync(Actividad actividad);
}
