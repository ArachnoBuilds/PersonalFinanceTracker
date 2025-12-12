using Application.Schema.BudgetTracking.Models;
using Application.Schema.Shared;

namespace Application.Schema.BudgetTracking.GetTransaction;

public record Query(int Year, int Month);

public interface IHandler
{
    Task<Result<List<TransactionInfo>>> DoAsync(Query query);
}
