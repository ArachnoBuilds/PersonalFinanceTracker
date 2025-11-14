using BPM = Application.Features.BudgetPlanning.Models;
using Application.Shared;
using Application.Shared.Models;
using Application.Shared.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.BudgetPlanning.GetCategoryDescription;

public class GetCategoryDescriptionHandler(ApplicationDbContext context)
{
    public async Task<Result<List<BPM.Category>>> DoAsync(GetCategoryDescriptionQuery query, CancellationToken cancellation = default)
    {
        if (query.Type is BudgetType.Summary)
            return Errors.BudgetTypeSummaryNotAllowed;

        var type = query.Type.ToString();
        List<BPM.Category> descriptions;
        try
        {
            descriptions = await context.Categories
                            .AsNoTracking()
                            .Where(p => p.Type == type)
                            .Select(p => new BPM.Category(p.Id, p.Description))
                            .ToListAsync(cancellation)
                            .ConfigureAwait(false);
        }
        catch (Exception exc)
        {
            return Result.Failure<List<BPM.Category>>(exc);
        }
        return descriptions;
    }
}
