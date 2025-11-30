using MyPlanU.Backend.Data.Repositories;
using MyPlanU.Backend.Models;

namespace MyPlanU.Backend.Business;

public class RecordatorioService
{
    private readonly IRecordatorioRepository _recordatorioRepository;

    public RecordatorioService(IRecordatorioRepository recordatorioRepository)
    {
        _recordatorioRepository = recordatorioRepository;
    }

    public Task SaveRecordatorioAsync(Recordatorio recordatorio)
    {
        return _recordatorioRepository.SaveRecordatorioAsync(recordatorio);
    }
}
