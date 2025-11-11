using ViewModels;

namespace Components.ViewModels;

public record BudgetSummary(
    Month Month,
    decimal Income,
    decimal Expenses,
    decimal Savings)
{
    public decimal Balance => Income - (Expenses + Savings);
}
