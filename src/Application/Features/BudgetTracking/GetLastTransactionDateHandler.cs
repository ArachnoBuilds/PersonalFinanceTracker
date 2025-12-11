using Application.Schema.BudgetTracking.GetLastTransactionDate;
using Application.Schema.Shared;
using Application.Shared.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.BudgetTracking;

public class GetLastTransactionDateHandler(ApplicationDbContext context) : IHandler
{
    public async Task<Result<DateTime>> DoAsync(CancellationToken cancellationToken = default)
    {
        string? lastTransactionDate;
        try
        {
            lastTransactionDate = await context.Transactions
                .OrderByDescending(t => t.Date)
                .Select(t => t.EffectiveDate)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception exc)
        {
            return Result.Failure<DateTime>(exc);
        }
        return Result.Success(string.IsNullOrEmpty(lastTransactionDate) 
            ? DateTime.Today 
            : DateTime.Parse(lastTransactionDate));
    }
}
