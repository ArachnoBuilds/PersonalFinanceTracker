using Application.Features.BudgetPlanning.Models;
using Application.Shared;
using Application.Shared.Models;
using Application.Shared.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.BudgetPlanning.GetBudget;
using CategoryBasedBudget = (int CategoryId, string CategoryDesc, IEnumerable<Budget> Budgets);

public class GetBudgetHandler(ApplicationDbContext context)
{
    public async Task<Result<List<AnnualBudget>>> DoAsync(GetBudgetQuery query, CancellationToken cancellation = default)
    {
        if (query.Type is BudgetType.Summary)
            return Errors.BudgetTypeSummaryNotAllowed;

        var (year, type) = (query.Year, query.Type.ToString());
        List<AnnualBudget> annualBudgets = [];
        try
        {
            // fetch budgets from db
            var budgets = await context.Categories
                        .Include(p => p.Budgets)
                        .AsNoTracking()
                        .Where(p => p.Type == type && p.Budgets.Any(b => b.Year == year))
                        .Select(p => new CategoryBasedBudget(
                            p.Id,
                            p.Description,
                            p.Budgets.Where(b => b.Year == year)))
                        .ToListAsync(cancellation)
                        .ConfigureAwait(false);

            // map to annual budgets
            annualBudgets = [
                .. budgets
                .Select(b => new AnnualBudget(
                    b.CategoryId,
                    b.CategoryDesc,
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
