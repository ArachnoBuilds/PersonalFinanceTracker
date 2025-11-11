namespace ViewModels;

public record AnnualBudget(string Category, Dictionary<Month, decimal> Budgets);

