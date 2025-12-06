using Application.Schema.BudgetPlanning.Models;
using Application.Schema.BudgetPlanning.UpdateBudget;
using Application.Schema.Shared;
using Application.Shared.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.BudgetPlanning;

public class UpdateBudgetHandler(ApplicationDbContext context): IHandler
{
    public async Task<Result> DoAsync(Command command, CancellationToken cancellation = default)
    {
        try
        {
            var budgetItem = context.Categories
                            .Include(p => p.Budgets.Where(b => b.Year == command.Year))
                            .FirstOrDefault(p => p.Id == command.Data.BudgetItemId);
            if (budgetItem == null)
                return Errors.BudgetCategoryNotFound;

            // update category description
            if (!budgetItem.Description.Equals(command.Data.BudgetItemDesc, StringComparison.InvariantCulture))
                budgetItem.Description = command.Data.BudgetItemDesc;

            // update budget amounts
            foreach (var p in budgetItem.Budgets)
                if (command.Data.MonthlyAmounts.TryGetValue((Month)p.Month, out decimal amt) && p.Amount != (double)amt)
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