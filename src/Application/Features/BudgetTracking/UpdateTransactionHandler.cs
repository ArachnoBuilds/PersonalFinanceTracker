using Application.Shared.Persistence;
using Application.Schema.BudgetTracking.UpdateTransaction;
using Application.Schema.Shared;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.BudgetTracking;

public class UpdateTransactionHandler(ApplicationDbContext context) : IHandler
{
    public async Task<Result> DoAsync(Command command, CancellationToken cancellationToken = default)
    {
        try
        {
            var transaction = await context.Transactions
                .FirstOrDefaultAsync(p => p.Id == command.Data.Id, cancellationToken)
                .ConfigureAwait(false);
            if (transaction == null)
                return Result.Failure(Errors.TransactionNotFound);

            var accExists = await context.Accounts
                    .AnyAsync(p => p.Value == command.Data.Account, cancellationToken)
                    .ConfigureAwait(false);
            if (!accExists)
                context.Accounts.Add(new Account
                {
                    Value = command.Data.Account
                });

            transaction.Date = command.Data.Date.ToString("o");
            transaction.BudgetId = command.Data.BudgetId;
            transaction.Amount = (double)command.Data.Amount;
            transaction.Description = command.Data.Description;
            transaction.EffectiveDate = command.Data.EffectiveDate.ToString("o");
            transaction.Account = command.Data.Account;

            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exc)
        {
            return Result.Failure(exc);
        }
        return Result.Success();
    }
}
