namespace Application.Schema.BudgetPlanning.Models;

public record Budget(
    int BudgetItemId, 
    string BudgetItemDesc, //TODO remove if not needed
    Dictionary<Month, decimal> MonthlyAmounts);