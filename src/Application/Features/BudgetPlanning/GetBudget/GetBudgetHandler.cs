using Application.Features.BudgetPlanning.GetBudget.Models;
using Application.Shared;
using Application.Shared.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.BudgetPlanning.GetBudget;
using CategoryBasedBudget = (string Category, IEnumerable<Budget> Budgets);

public class GetBudgetHandler(ApplicationDbContext context)
{
    public async Task<Result<List<AnnualBudget>>> DoAsync(GetBudgetQuery query, CancellationToken cancellation = default)
    {
        var (year, type) = (query.Year, query.Type.ToString());
        List<AnnualBudget> annualBudgets = [];
        try
        {
            // fetch budgets from db
            var budgets = await context.Categories
                        .AsNoTracking()
                        .Where(p => p.Type == type)
                        .Select(p => new CategoryBasedBudget(
                            p.Description,
                            p.Budgets.Where(b => b.Year == year)))
                        .ToListAsync(cancellation)
                        .ConfigureAwait(false);

            // map to annual budgets
            annualBudgets = [
                .. budgets
                .Select(b => new AnnualBudget(
                    b.Category,
                    b.Budgets.ToDictionary(
                        k => (Month)k.Month,
                        v => (decimal)v.Amount)))
            ];
        }
        catch (Exception exc)
        {
            return Result.Failure<List<AnnualBudget>>(exc);
        }
        return annualBudgets;
    }
}
