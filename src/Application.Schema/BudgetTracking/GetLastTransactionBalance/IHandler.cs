using Application.Schema.Shared;

namespace Application.Schema.BudgetTracking.GetLastTransactionBalance;

public interface IHandler
{
    Task<Result<decimal>> DoAsync(CancellationToken cancellationToken = default);
}
