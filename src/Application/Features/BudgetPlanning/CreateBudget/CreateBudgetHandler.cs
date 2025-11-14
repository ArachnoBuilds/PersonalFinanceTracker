using Application.Shared;
using Application.Shared.Persistence;

namespace Application.Features.BudgetPlanning.CreateBudget;

public class CreateBudgetHandler(ApplicationDbContext context)
{
    public async Task<Result> DoAsync(CreateBudgetCommand command)
    {
        try
        {
            if (command.Budget.CategoryId == -1)
            {
                // fetch counts of existing categories
                var count = context.Categories.Count();

                // create new category and budgets
                var category = command.Budget.ToCategory(command.Type, count + 1);
                var budgets = command.Budget.ToBudgets(command.Year, category.Id);

                // add to context
                context.Categories.Add(category);
                context.Budgets.AddRange(budgets);
            }
            else
            {
                // create budgets only
                var budgets = command.Budget.ToBudgets(command.Year);
                context.Budgets.AddRange(budgets);
            }
            await context.SaveChangesAsync().ConfigureAwait(false);
        }
        catch (Exception exc)
        {
            return Result.Failure(exc);
        }
        return Result.Success();
    }
}
