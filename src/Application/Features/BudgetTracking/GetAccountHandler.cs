using Application.Schema.BudgetTracking.GetAccount;
using Application.Schema.Shared;
using Application.Shared.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.BudgetTracking;

public class GetAccountHandler(ApplicationDbContext context) : IHandler
{
    public async Task<Result<List<string>>> DoAsync(CancellationToken cancellationToken = default)
    {
		List<string> accounts = [];
		try
		{
			accounts = await context.Accounts
						.AsNoTracking()
						.Select(p => p.Value)
						.ToListAsync(cancellationToken)
						.ConfigureAwait(false);
		}
		catch (Exception exc)
		{
			return Result.Failure<List<string>>(exc);
		}
		return Result.Success(accounts);
    }
}
