using Application.Schema.Shared;

namespace Application.Schema.BudgetTracking.GetTransactionCount;

public record Query(string Year);

public interface IHandler
{
    Task<Result<int>> DoAsync(CancellationToken cancellationToken = default);
    Task<Result<int>> DoAsync(Query query, CancellationToken cancellationToken = default);
}
