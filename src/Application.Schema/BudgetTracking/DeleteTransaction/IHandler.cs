using Application.Schema.Shared;

namespace Application.Schema.BudgetTracking.DeleteTransaction;

public record Command(string TransactionId);

public interface IHandler
{
    Task<Result> DoAsync(Command command, CancellationToken cancellationToken = default);
}
