using Application.Schema.BudgetTracking.Models;
using Application.Schema.Shared;

namespace Application.Schema.BudgetTracking.UpdateTransaction;

public record Command(Transaction Data);

public interface IHandler
{
    Task<Result> DoAsync(Command command, CancellationToken cancellationToken = default);
}
