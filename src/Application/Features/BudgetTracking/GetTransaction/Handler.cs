using Application.Shared;
using Application.Shared.Models;
using Application.Shared.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.BudgetTracking.GetTransaction;

public class Handler(ApplicationDbContext context)
{
    public async Task<Result<List<Models.Transaction>>> DoAsync(Query query)
    {
        var (year, month) = query;
        List<Models.Transaction> transactions = [];
        try
        {
            // fetch from db
            var data = await context.Transactions
                .AsNoTracking()
                .Include(p => p.Budget)
                .ThenInclude(b => b.Category)
                .Where(p =>
                    p.EffectiveDate.Substring(0, 4) == year &&
                    p.EffectiveDate.Substring(5, 2) == month)
                .OrderBy(p => p.Date)
                .ToListAsync()
                .ConfigureAwait(false);

            // map
            transactions =
            [
                ..data.Select(p => new Models.Transaction(
                p.Id,
                DateTime.Parse(p.Date),
                new(
                    p.Budget.Id,
                    Enum.Parse<BudgetType>(p.Budget.Category.Type),
                    new(p.Budget.Category.Id, p.Budget.Category.Description)),
                p.Account,
                (decimal)p.Amount,
                p.Description,
                DateTime.Parse(p.EffectiveDate)))
            ];
        }
        catch (Exception exc)
        {
            return Result.Failure<List<Models.Transaction>>(exc);
        }
        return transactions;
    }
}