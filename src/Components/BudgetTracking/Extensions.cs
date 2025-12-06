using BT = Application.Features.BudgetTracking;

namespace Components.BudgetTracking;

internal static class Extensions
{
    extension(BT.Models.Transaction transaction)
    {
        public Models.Transaction ToTransaction() => new()
        {
            Id = transaction.Id,
            Date = transaction.Date,
            BudgetType = transaction.BudgetInfo.Type,
            BudgetCategory = transaction.BudgetInfo.Category,
            Account = transaction.Account,
            Amount = transaction.Amount,
            Description = transaction.Description,
            EffectiveDate = transaction.EffectiveDate
        };
    }
}
