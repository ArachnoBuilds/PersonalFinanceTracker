using Application.Schema.Shared;

namespace Application.Schema.BudgetPlanning.DeleteBudget;

public record Command(int Year, int BudgetItemId);

public interface IHandler
{
    Task<Result> DoAsync(Command command, CancellationToken cancellation = default);
}
