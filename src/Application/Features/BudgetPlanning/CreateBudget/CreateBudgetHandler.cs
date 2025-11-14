using Application.Shared;
using Application.Shared.Persistence;

namespace Application.Features.BudgetPlanning.CreateBudget;

public class CreateBudgetHandler(ApplicationDbContext context)
{
    public async Task<Result> DoAsync(CreateBudgetCommand command)
    {
        return Result.Failure(Error.NullValue);
    }
}
