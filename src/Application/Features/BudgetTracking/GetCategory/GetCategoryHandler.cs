using SM = Application.Shared.Models;
using Application.Shared;
using Application.Shared.Models;
using Application.Shared.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.BudgetTracking.GetCategory;

public class GetCategoryHandler(ApplicationDbContext context)
{
    public async Task<Result<List<SM.Category>>> DoAsync(GetCategoryQuery query, CancellationToken cancellation = default)
    {
        if (query.Type is BudgetType.Summary)
            return Errors.BudgetTypeSummaryNotAllowed;

        var (type, year) = (query.Type.ToString(), query.Year);
        List<SM.Category> categories;
        try
        {
            categories = await context.Categories
                            .Include(p => p.Budgets.Where(b => b.Year == year))
                            .AsNoTracking()
                            .Where(p => p.Type == type && p.Budgets.Any())
                            .Select(p => new SM.Category(p.Id, p.Description))
                            .ToListAsync(cancellation)
                            .ConfigureAwait(false);
        }
        catch (Exception exc)
        {
            return Result.Failure<List<SM.Category>>(exc);
        }
        return categories;
    }
}
