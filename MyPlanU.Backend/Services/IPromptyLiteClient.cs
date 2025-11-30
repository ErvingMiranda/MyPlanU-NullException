using System.Threading;
using System.Threading.Tasks;

namespace MyPlanU.Backend.Services;

public interface IPromptyLiteClient
{
    Task<string> AskAsync(string userMessage, CancellationToken ct = default);
}
