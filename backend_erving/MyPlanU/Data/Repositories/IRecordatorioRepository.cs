using MyPlanU.Models;

namespace MyPlanU.Data.Repositories;

public interface IRecordatorioRepository
{
    Task<int> CreateAsync(Recordatorio recordatorio);
    Task<int> UpdateAsync(Recordatorio recordatorio);
    Task<int> DeleteAsync(int idRecordatorio);

    Task<Recordatorio?> GetByIdAsync(int idRecordatorio);

    Task<List<Recordatorio>> GetActivosPendientesAsync(DateTime hasta);
}
