using Application.Schema.BudgetTracking.GetLastTransactionBalance;
using Application.Schema.Shared;
using Application.Schema.Shared.Models;
using Application.Shared.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.BudgetTracking;

public class GetLastTransactionBalanceHandler(ApplicationDbContext context) : IHandler
{
    public async Task<Result<decimal>> DoAsync(CancellationToken cancellationToken = default)
    {
        var incomeBudgetItemType = BudgetItemType.Income.ToString();
        decimal balance;
        try
        {
            balance = (decimal)(await context.Transactions
                .AsNoTracking()
                .Include(p => p.Budget)
                .ThenInclude(p => p.BudgetItem)
                .SumAsync(p => p.Budget.BudgetItem.Type == incomeBudgetItemType
                    ? p.Amount
                    : -p.Amount,
                    cancellationToken)
                .ConfigureAwait(false));
        }
        catch (Exception exc)
        {
            return Result.Failure<decimal>(exc);
        }
        return Result.Success(balance);
    }
}
