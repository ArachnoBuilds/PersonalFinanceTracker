namespace Application.Features.BudgetPlanning.GetBudget.Models;

public record AnnualBudget(string Category, Dictionary<Month, decimal> Budgets);

