using Application.Schema.BudgetTracking.GetBudget;
using BTM = Application.Schema.BudgetTracking.Models;
using Application.Schema.Shared;
using Application.Schema.Shared.Models;
using ASP = Application.Shared.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.BudgetTracking;

public class GetBudgetHandler(ASP.ApplicationDbContext context): IHandler
{
    public async Task<Result<List<BTM.Budget>>> DoAsync(Query query, CancellationToken cancellation = default)
    {
        if (query.Type is BudgetItemType.Summary)
            return Errors.BudgetTypeSummaryNotAllowed;

        var (type, year, month) = (query.Type.ToString(), query.Year, query.Month);
        List<BTM.Budget> budgets;
        try
        {
            budgets = await context.Budgets
                            .AsNoTracking()
                            .Include(p => p.BudgetItem)
                            .Where(p => p.BudgetItem.Type == type && p.Year == year && p.Month == month)
                            .Select(p => new BTM.Budget(p.Id, p.BudgetItem.Description))
                            .ToListAsync(cancellation)
                            .ConfigureAwait(false);
        }
        catch (Exception exc)
        {
            return Result.Failure<List<BTM.Budget>>(exc);
        }
        return budgets;
    }
}
