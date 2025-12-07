using Application.Schema.BudgetTracking.DeleteTransaction;
using Application.Schema.Shared;
using Application.Shared.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.BudgetTracking;

public class DeleteTransactionHandler(ApplicationDbContext context) : IHandler
{
    public async Task<Result> DoAsync(Command command, CancellationToken cancellationToken = default)
    {
        try
        {
            var transaction = await context.Transactions
                    .FirstOrDefaultAsync(p => p.Id == command.TransactionId, cancellationToken)
                    .ConfigureAwait(false);
            if (transaction == null)
                return Result.Failure(Errors.TransactionNotFound);

            context.Transactions.Remove(transaction);

            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exc)
        {
            return Result.Failure(exc);
        }
        return Result.Success();
    }
}
