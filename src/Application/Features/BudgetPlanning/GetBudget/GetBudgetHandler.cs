using Application.Features.BudgetPlanning.GetBudget.Models;
using Application.Shared;
using Application.Shared.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.BudgetPlanning.GetBudget;

public class GetBudgetHandler(ApplicationDbContext context)
{
    public async Task<Result<List<AnnualBudget>>> DoAsync(GetBudgetQuery query, CancellationToken cancellation = default)
    {
        var (year, type) = (query.Year, query.Type.ToString());
        List<AnnualBudget> budgets = [];
        try
        {
            budgets = await context.Categories
                        .AsNoTracking()
                        .Where(p => p.Type == type)
                        .Select(p => new AnnualBudget(
                            p.Description,
                            p.Budgets
                                .Where(b => b.Year == year)
                                .ToDictionary(
                                    b => (Month)b.Month,
                                    b => (decimal)b.Amount)))
                        .ToListAsync(cancellation)
                        .ConfigureAwait(false);
        }
        catch (Exception exc)
        {
            return Result.Failure<List<AnnualBudget>>(exc);
        }
        return budgets;
    }
}
