namespace Application.Features.BudgetPlanning.Models;

public record AnnualBudget(string Category, Dictionary<Month, decimal> Budgets);

