using Application.Schema.BudgetTracking.GetTransaction;
using Application.Schema.BudgetTracking.Models;
using Application.Schema.Shared;
using Application.Shared.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.BudgetTracking;

public class GetTransactionHandler(ApplicationDbContext context) : IHandler
{
    public async Task<Result<List<TransactionInfo>>> DoAsync(Query query)
    {
        var (year, month) = query;
        List<TransactionInfo> transactions = [];
        try
        {
            // fetch from db
            var data = await context.Transactions
                .AsNoTracking()
                .Include(p => p.Budget)
                .ThenInclude(b => b.Category)
                .Where(p =>
                    p.EffectiveDate.Substring(6, 4) == year &&
                    p.EffectiveDate.Substring(3, 2) == month)
                .OrderBy(p => p.Date)
                .ToListAsync()
                .ConfigureAwait(false);

            // map
            transactions =
            [
                ..data.Select(p => new TransactionInfo()
                {
                    Id = p.Id,
                    Date = DateTime.Parse(p.Date),
                    Budget = new(
                        p.Budget.Id,
                        p.Budget.Category.Description),
                    Account = p.Account,
                    Amount = (decimal)p.Amount,
                    Description = p.Description,
                    EffectiveDate = DateTime.Parse(p.EffectiveDate)
                })
            ];
        }
        catch (Exception exc)
        {
            return Result.Failure<List<TransactionInfo>>(exc);
        }
        return transactions;
    }
}