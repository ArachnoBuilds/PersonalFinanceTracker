using Application.Schema.BudgetTracking.Models;
using Application.Schema.Shared;
using Application.Schema.Shared.Models;

namespace Application.Schema.BudgetTracking.GetBudget;

public record Query(BudgetItemType Type, int Year, int Month);

public interface IHandler
{
    Task<Result<List<Budget>>> DoAsync(Query query, CancellationToken cancellation = default);
}
