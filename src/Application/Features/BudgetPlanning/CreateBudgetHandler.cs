using Application.Schema.BudgetPlanning.CreateBudget;
using Application.Schema.Shared;
using Application.Shared.Persistence;

namespace Application.Features.BudgetPlanning;

public class CreateBudgetHandler(ApplicationDbContext context) : IHandler
{
    public async Task<Result<int>> DoAsync(Command command, CancellationToken cancellationToken = default)
    {
        int categoryId;
        try
        {
            if (command.Budget.BudgetItemId == -1)
            {
                // fetch counts of existing categories
                var count = context.Categories.Count();

                // create new category and budgets
                categoryId = count + 1;
                var category = command.Budget.ToCategory(command.Type, categoryId);
                var budgets = command.Budget.ToBudgets(command.Year, category.Id);

                // add to context
                context.Categories.Add(category);
                context.Budgets.AddRange(budgets);
            }
            else
            {
                categoryId = command.Budget.BudgetItemId;

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
        return Result.Success(categoryId);
    }
}
