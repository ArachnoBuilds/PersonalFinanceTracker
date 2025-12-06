using Application.Schema.BudgetTracking.CreateTransaction;
using Application.Schema.Shared;
using Application.Shared.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.BudgetTracking;

public class CreateTransactionHandler(ApplicationDbContext context): IHandler
{
    public async Task<Result> DoAsync(Command command)
    {
        try
        {
            var accExists = await context.Accounts
                .AnyAsync(p => p.Value == command.Data.Account)
                .ConfigureAwait(false);
            if (!accExists)
                context.Accounts.Add(new Account
                {
                    Value = command.Data.Account
                });

            context.Transactions.Add(new()
            {
                Id = command.Data.Id,
                Date = command.Data.Date.ToString("d"),
                BudgetId = command.Data.BudgetId,
                Amount = (double)command.Data.Amount,
                Description = command.Data.Description,
                EffectiveDate = command.Data.EffectiveDate.ToString("d"),
                Account = command.Data.Account
            });
            
            await context.SaveChangesAsync().ConfigureAwait(false);
        }
        catch (Exception exc)
        {
            return Result.Failure(exc);
        }
        return Result.Success();
    }
}


