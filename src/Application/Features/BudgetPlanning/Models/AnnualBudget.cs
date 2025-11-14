namespace Application.Features.BudgetPlanning.Models;

public record AnnualBudget(int CategoryId, string CategoryDesc, Dictionary<Month, decimal> Budgets);