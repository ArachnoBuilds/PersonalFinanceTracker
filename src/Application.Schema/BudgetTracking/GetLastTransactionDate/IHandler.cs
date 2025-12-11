using Application.Schema.Shared;

namespace Application.Schema.BudgetTracking.GetLastTransactionDate;

public interface IHandler
{
    Task<Result<DateTime>> DoAsync(CancellationToken cancellationToken = default);
}
