using MyPlanU.Backend.Models;

namespace MyPlanU.Backend.Data.Repositories;

public interface IRecordatorioRepository
{
    Task<Recordatorio> GetRecordatorioAsync(int id);
    Task<int> SaveRecordatorioAsync(Recordatorio recordatorio);
    Task<int> DeleteRecordatorioAsync(Recordatorio recordatorio);
}
