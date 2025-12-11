using Application.Schema.BudgetTracking.GetTransactionCount;
using Application.Schema.Shared;
using Application.Shared.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.BudgetTracking;

public class GetTransactionCountHandler(ApplicationDbContext context) : IHandler
{
    public async Task<Result<int>> DoAsync(CancellationToken cancellationToken = default)
    {
        int count;
        try
        {
            count = await context.Transactions.CountAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exc)
        {
            return Result.Failure<int>(exc);
        }
        return Result.Success(count);
    }

    public async Task<Result<int>> DoAsync(Query query, CancellationToken cancellationToken = default)
    {
        int count;
        try
        {
            count = await context.Transactions
                .Where(p => p.Date.Substring(6, 4) == query.Year)
                .CountAsync(cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception exc)
        {
            return Result.Failure<int>(exc);
        }
        return Result.Success(count);
    }
}