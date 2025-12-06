using Application.Schema.BudgetTracking.Models;
using Application.Schema.Shared;

namespace Application.Schema.BudgetTracking.CreateTransaction;

public record Command(Transaction Data);

public interface IHandler
{
    Task<Result> DoAsync(Command command);
}
