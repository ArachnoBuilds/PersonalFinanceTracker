using Application.Schema.BudgetTracking.Models;

namespace Components.BudgetTracking;

internal static class Extensions
{
    extension(TransactionInfo transaction)
    {
        public Transaction ToTransaction() => new(
            transaction.Id,
            transaction.Date,
            transaction.Budget.Id,
            transaction.Account,
            transaction.Amount,
            transaction.Description,
            transaction.EffectiveDate);
    }
}
