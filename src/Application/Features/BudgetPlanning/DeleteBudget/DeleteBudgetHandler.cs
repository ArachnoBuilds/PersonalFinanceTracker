using Application.Shared;
using Application.Shared.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.BudgetPlanning.DeleteBudget;

public class DeleteBudgetHandler(ApplicationDbContext context)
{
    public async Task<Result> DoAsync(DeleteBudgetCommand command, CancellationToken cancellation = default)
    {
        try
        {
            var category = context.Categories
                            .Include(p => p.Budgets.Where(b => b.Year == command.Year))
                            .FirstOrDefault(p => p.Id == command.CategoryId);
            if (category == null)
                return Errors.BudgetCategoryNotFound;

            // remove budget entries for the specified year
            context.Budgets.RemoveRange(category.Budgets);

            await context.SaveChangesAsync(cancellation).ConfigureAwait(false);
        }
        catch (Exception exc)
        {
            Result.Failure(exc);
        }
        return Result.Success();
    }
}