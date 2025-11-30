using MyPlanU.Backend.Models;

namespace MyPlanU.Backend.Services;

public interface IPromptyClient
{
    Task<PromptyChatResponse> EnviarMensajeAsync(
        string mensaje,
        List<PromptyHistoryItem>? historial = null,
        CancellationToken cancellationToken = default);
}
