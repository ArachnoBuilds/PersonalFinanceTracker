using Application.Schema.BudgetPlanning.DeleteBudget;
using Application.Schema.Shared;
using Application.Shared.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.BudgetPlanning;

public class DeleteBudgetHandler(ApplicationDbContext context): IHandler
{
    public async Task<Result> DoAsync(Command command, CancellationToken cancellation = default)
    {
        try
        {
            var budgetItems = context.BudgetItems
                            .Include(p => p.Budgets.Where(b => b.Year == command.Year))
                            .FirstOrDefault(p => p.Id == command.BudgetItemId);
            if (budgetItems == null)
                return Errors.BudgetItemNotFound;

            // remove budget entries for the specified year
            context.Budgets.RemoveRange(budgetItems.Budgets);

            await context.SaveChangesAsync(cancellation).ConfigureAwait(false);
        }
        catch (Exception exc)
        {
            Result.Failure(exc);
        }
        return Result.Success();
    }
}