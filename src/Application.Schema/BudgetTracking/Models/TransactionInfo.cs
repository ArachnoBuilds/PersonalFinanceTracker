using Application.Schema.Shared.Models;

namespace Application.Schema.BudgetTracking.Models;

public record TransactionInfo
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime Date { get; set;  } = DateTime.Now;
    public BudgetItemType BudgetType { get; set; } = BudgetItemType.Income;
    public Budget Budget { get; set; } = new(Guid.Empty.ToString(), string.Empty);
    public decimal Amount { get; set; } = 0.00m;
    public string Account { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Balance { get; set; } = 0.00m;
    public DateTime EffectiveDate { get; set; } = DateTime.Now;
}