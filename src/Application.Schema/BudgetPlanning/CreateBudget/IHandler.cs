using Application.Schema.BudgetPlanning.Models;
using Application.Schema.Shared;
using Application.Schema.Shared.Models;

namespace Application.Schema.BudgetPlanning.CreateBudget;

public record Command(int Year, BudgetItemType Type, Budget Budget);

public interface IHandler
{
    Task<Result<int>> DoAsync(Command command, CancellationToken cancellationToken = default);
}