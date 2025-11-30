using System.Threading;
using System.Threading.Tasks;

namespace MyPlanU.Backend.Services;

public interface IPromptyLiteClient
{
    Task<string> AskAsync(string userMessage, CancellationToken ct = default);
    Task<bool> CheckHealthAsync(CancellationToken ct = default);
    Task<bool> HasValidTokenAsync(CancellationToken ct = default);
    Task<bool> SetTokenAsync(string token, CancellationToken ct = default);
}

