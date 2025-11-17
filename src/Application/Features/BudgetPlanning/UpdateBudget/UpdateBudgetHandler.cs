using Application.Features.BudgetPlanning.Models;
using Application.Shared;
using Application.Shared.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.BudgetPlanning.UpdateBudget;

public class UpdateBudgetHandler(ApplicationDbContext context)
{
    public async Task<Result> DoAsync(UpdateBudgetCommand command, CancellationToken cancellation = default)
    {
        try
        {
            var category = context.Categories
                            .Include(p => p.Budgets.Where(b => b.Year == command.Year))
                            .FirstOrDefault(p => p.Id == command.Budget.CategoryId);
            if (category == null)
                return Errors.BudgetCategoryNotFound;

            // update category description
            if (!category.Description.Equals(command.Budget.CategoryDesc, StringComparison.InvariantCulture))
                category.Description = command.Budget.CategoryDesc;

            // update budget amounts
            foreach (var p in category.Budgets)
                if (command.Budget.Budgets.TryGetValue((Month)p.Month, out decimal amt) && p.Amount != (double)amt)
                    p.Amount = (double)amt;

            await context.SaveChangesAsync(cancellation).ConfigureAwait(false);
        }
        catch (Exception exc)
        {
            Result.Failure(exc);
        }
        return Result.Success();
    }
}