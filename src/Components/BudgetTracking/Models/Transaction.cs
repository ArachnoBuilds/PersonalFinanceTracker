using Application.Shared.Models;

namespace Components.BudgetTracking.Models;

public record Transaction
{
    public string Id { get; set; } = string.Empty;
    public DateTime Date { get; set;  } = DateTime.Now;
    public BudgetType BudgetType { get; set; } = BudgetType.Income;
    public Category BudgetCategory { get; set; } = new(-1, string.Empty);
    public decimal Amount { get; set; } = 0.00m;
    public string Account { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Balance { get; set; } = 0.00m;
    public DateTime EffectiveDate { get; set; } = DateTime.Now;
}
