using Application.Schema.Shared;
using Application.Schema.Shared.Models;

namespace Application.Schema.BudgetPlanning.GetBudgetItem;

public record Query(BudgetItemType Type);

public interface IHandler
{
    Task<Result<List<BudgetItem>>> DoAsync(Query query, CancellationToken cancellation = default);
}
