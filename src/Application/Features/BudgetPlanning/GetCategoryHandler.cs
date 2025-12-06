using Application.Shared.Persistence;
using Microsoft.EntityFrameworkCore;
using Application.Schema.Shared;
using Application.Schema.Shared.Models;
using Application.Schema.BudgetPlanning.GetBudgetItem;

namespace Application.Features.BudgetPlanning;

public class GetCategoryHandler(ApplicationDbContext context): IHandler
{
    public async Task<Result<List<BudgetItem>>> DoAsync(Query query, CancellationToken cancellation = default)
    {
        if (query.Type is BudgetItemType.Summary)
            return Errors.BudgetTypeSummaryNotAllowed;

        var type = query.Type.ToString();
        List<BudgetItem> budgetItems;
        try
        {
            budgetItems = await context.Categories
                            .AsNoTracking()
                            .Where(p => p.Type == type)
                            .Select(p => new BudgetItem(p.Id, p.Description))
                            .ToListAsync(cancellation)
                            .ConfigureAwait(false);
        }
        catch (Exception exc)
        {
            return Result.Failure<List<BudgetItem>>(exc);
        }
        return budgetItems;
    }
}
