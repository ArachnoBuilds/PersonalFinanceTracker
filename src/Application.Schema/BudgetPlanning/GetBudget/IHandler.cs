using Application.Schema.BudgetPlanning.Models;
using Application.Schema.Shared;
using Application.Schema.Shared.Models;

namespace Application.Schema.BudgetPlanning.GetBudget;

public record Query(int Year, BudgetItemType Type);

public interface IHandler
{
    Task<Result<List<BudgetInfo>>> DoAsync(Query query, CancellationToken cancellation = default);
}
