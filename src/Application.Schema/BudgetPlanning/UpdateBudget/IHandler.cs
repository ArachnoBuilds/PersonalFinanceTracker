using Application.Schema.BudgetPlanning.Models;
using Application.Schema.Shared;

namespace Application.Schema.BudgetPlanning.UpdateBudget;

public record Command(int Year, Budget Data);

public interface IHandler
{
    Task<Result> DoAsync(Command command, CancellationToken cancellation = default);
}
