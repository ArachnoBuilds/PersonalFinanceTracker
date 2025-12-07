using Application.Schema.Shared;

namespace Application.Schema.BudgetTracking.GetAccount;

public interface IHandler
{
    Task<Result<List<string>>> DoAsync(CancellationToken cancellationToken = default);
}
