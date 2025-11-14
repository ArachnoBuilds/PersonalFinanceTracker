namespace Application.Features.BudgetPlanning.Models;

public record AnnualBudget(string CategoryDesc, Dictionary<Month, decimal> Budgets);

