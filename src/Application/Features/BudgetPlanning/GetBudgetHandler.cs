using Application.Schema.BudgetPlanning.GetBudget;
using Application.Schema.BudgetPlanning.Models;
using Application.Schema.Shared;
using Application.Schema.Shared.Models;
using Application.Shared.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.BudgetPlanning;
using Budget = (int BudgetItemId, string BudgetItemDesc, IEnumerable<Shared.Persistence.Budget> Budgets);

public class GetBudgetHandler(ApplicationDbContext context): IHandler
{
    public async Task<Result<List<BudgetInfo>>> DoAsync(Query query, CancellationToken cancellation = default)
    {
        if (query.Type is BudgetItemType.Summary)
            return Errors.BudgetTypeSummaryNotAllowed;

        var (year, type) = (query.Year, query.Type.ToString());
        List<BudgetInfo> budgets = [];
        try
        {
            // fetch budgets from db
            var data = await context.BudgetItems
                .AsNoTracking()
                .Include(p => p.Budgets.Where(b => b.Year == year))
                .Where(p => p.Type == type && p.Budgets.Any())
                .Select(p => new Budget(
                    p.Id,
                    p.Description,
                    p.Budgets))
                .ToListAsync(cancellation)
                .ConfigureAwait(false);

            // map to budgets
            foreach (var p in data)
                budgets.Add(new()
                {
                    BudgetItemId = p.BudgetItemId,
                    BudgetItemDesc = p.BudgetItemDesc,
                    Jan = (decimal)(p.Budgets.FirstOrDefault(b => b.Month == 1)?.Amount ?? 0),
                    Feb = (decimal)(p.Budgets.FirstOrDefault(b => b.Month == 2)?.Amount ?? 0),
                    Mar = (decimal)(p.Budgets.FirstOrDefault(b => b.Month == 3)?.Amount ?? 0),
                    Apr = (decimal)(p.Budgets.FirstOrDefault(b => b.Month == 4)?.Amount ?? 0),
                    May = (decimal)(p.Budgets.FirstOrDefault(b => b.Month == 5)?.Amount ?? 0),
                    Jun = (decimal)(p.Budgets.FirstOrDefault(b => b.Month == 6)?.Amount ?? 0),
                    Jul = (decimal)(p.Budgets.FirstOrDefault(b => b.Month == 7)?.Amount ?? 0),
                    Aug = (decimal)(p.Budgets.FirstOrDefault(b => b.Month == 8)?.Amount ?? 0),
                    Sep = (decimal)(p.Budgets.FirstOrDefault(b => b.Month == 9)?.Amount ?? 0),
                    Oct = (decimal)(p.Budgets.FirstOrDefault(b => b.Month == 10)?.Amount ?? 0),
                    Nov = (decimal)(p.Budgets.FirstOrDefault(b => b.Month == 11)?.Amount ?? 0),
                    Dec = (decimal)(p.Budgets.FirstOrDefault(b => b.Month == 12)?.Amount ?? 0)
                });
            budgets.Add(new()
            {
                BudgetItemId = -1,
                BudgetItemDesc = "Total",
                Jan = budgets.Sum(p => p.Jan),
                Feb = budgets.Sum(p => p.Feb),
                Mar = budgets.Sum(p => p.Mar),
                Apr = budgets.Sum(p => p.Apr),
                May = budgets.Sum(p => p.May),
                Jun = budgets.Sum(p => p.Jun),
                Jul = budgets.Sum(p => p.Jul),
                Aug = budgets.Sum(p => p.Aug),
                Sep = budgets.Sum(p => p.Sep),
                Oct = budgets.Sum(p => p.Oct),
                Nov = budgets.Sum(p => p.Nov),
                Dec = budgets.Sum(p => p.Dec),
            });
        }
        catch (Exception exc)
        {
            return Result.Failure<List<BudgetInfo>>(exc);
        }
        return budgets;
    }
}
