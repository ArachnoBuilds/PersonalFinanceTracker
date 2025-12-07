using Application.Schema.BudgetPlanning.CreateBudget;
using Application.Schema.Shared;
using Application.Shared.Persistence;

namespace Application.Features.BudgetPlanning;

public class CreateBudgetHandler(ApplicationDbContext context) : IHandler
{
    public async Task<Result<int>> DoAsync(Command command, CancellationToken cancellationToken = default)
    {
        int budgetItemId;
        try
        {
            if (command.Budget.BudgetItemId == -1)
            {
                // fetch counts of existing categories
                var count = context.BudgetItems.Count();

                // create new category and budgets
                budgetItemId = count + 1;
                var budgetItems = command.Budget.ToBudgetItem(command.Type, budgetItemId);
                var budgets = command.Budget.ToBudgets(command.Year, budgetItems.Id);

                // add to context
                context.BudgetItems.Add(budgetItems);
                context.Budgets.AddRange(budgets);
            }
            else
            {
                budgetItemId = command.Budget.BudgetItemId;

                // create budgets only
                var budgets = command.Budget.ToBudgets(command.Year);
                context.Budgets.AddRange(budgets);
            }
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exc)
        {
            return Result.Failure<int>(exc);
        }
        return Result.Success(budgetItemId);
    }
}
