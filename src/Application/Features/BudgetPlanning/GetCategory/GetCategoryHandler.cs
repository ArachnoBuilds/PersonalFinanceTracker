using SM = Application.Shared.Models;
using Application.Shared;
using Application.Shared.Models;
using Application.Shared.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.BudgetPlanning.GetCategory;

public class GetCategoryHandler(ApplicationDbContext context)
{
    public async Task<Result<List<SM.Category>>> DoAsync(GetCategoryQuery query, CancellationToken cancellation = default)
    {
        if (query.Type is BudgetType.Summary)
            return Errors.BudgetTypeSummaryNotAllowed;

        var type = query.Type.ToString();
        List<SM.Category> descriptions;
        try
        {
            descriptions = await context.Categories
                            .AsNoTracking()
                            .Where(p => p.Type == type)
                            .Select(p => new SM.Category(p.Id, p.Description))
                            .ToListAsync(cancellation)
                            .ConfigureAwait(false);
        }
        catch (Exception exc)
        {
            return Result.Failure<List<SM.Category>>(exc);
        }
        return descriptions;
    }
}
