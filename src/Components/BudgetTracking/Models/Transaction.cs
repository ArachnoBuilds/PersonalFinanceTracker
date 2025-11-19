using Application.Shared.Models;

namespace Components.BudgetTracking.Models;

public record Transaction(
    DateTime Date,
    BudgetType BudgetType,
    string BudgetCategory,
    decimal Amount,
    string Account,
    string Description)
{
    public decimal Balance { get; set; } = 0.00m;
    public DateTime EffectiveDate { get; set; } = Date;
}
