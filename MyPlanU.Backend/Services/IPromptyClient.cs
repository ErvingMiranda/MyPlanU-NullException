using MyPlanU.Backend.Models;

namespace MyPlanU.Backend.Services;

public interface IPromptyClient
{
    Task<PromptyChatResponse> EnviarMensajeAsync(
        string mensaje,
        List<PromptyMensaje>? historial = null,
        CancellationToken cancellationToken = default);
}
