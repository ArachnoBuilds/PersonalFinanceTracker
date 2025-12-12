using Application.Schema.BudgetTracking.GetTransaction;
using Application.Schema.BudgetTracking.Models;
using Application.Schema.Shared;
using Application.Schema.Shared.Models;
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
            // fetch closing balance
            var firstDay = new DateTime(year, month, 1, 0, 0, 0).ToString("o");
            var incomeBudgetItemType = BudgetItemType.Income.ToString();
            var closingBalance = await context.Transactions
                .AsNoTracking()
                .Include(p => p.Budget)
                .ThenInclude(p => p.BudgetItem)
                .Where(p => string.Compare(p.EffectiveDate, firstDay) < 0)
                .SumAsync(p => p.Budget.BudgetItem.Type == incomeBudgetItemType
                    ? p.Amount
                    : -p.Amount)
                .ConfigureAwait(false);

            // fetch from db
            var data = await context.Transactions
                .AsNoTracking()
                .Include(p => p.Budget)
                .ThenInclude(b => b.BudgetItem)
                .Where(p =>
                    p.EffectiveDate.Substring(0, 4) == $"{year}" &&
                    p.EffectiveDate.Substring(5, 2) == $"{month:00}")
                .OrderByDescending(p => p.Date)
                .ToListAsync()
                .ConfigureAwait(false);

            // map
            var balance = closingBalance;
            transactions =
            [
                ..data.Select(p => 
                {
                    balance += p.Budget.BudgetItem.Type == incomeBudgetItemType
                        ? p.Amount
                        : -p.Amount;
                    return new TransactionInfo()
                    {
                        Id = p.Id,
                        Date = DateTime.Parse(p.Date),
                        BudgetType = Enum.Parse<BudgetItemType>(p.Budget.BudgetItem.Type),
                        Budget = new(
                            p.Budget.Id,
                            p.Budget.BudgetItem.Description),
                        Account = p.Account,
                        Amount = (decimal)p.Amount,
                        Balance = (decimal)balance,
                        Description = p.Description,
                        EffectiveDate = DateTime.Parse(p.EffectiveDate)
                    };
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