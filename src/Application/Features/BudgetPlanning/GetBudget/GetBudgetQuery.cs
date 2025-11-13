using Application.Features.BudgetPlanning.GetBudget.Models;

namespace Application.Features.BudgetPlanning.GetBudget;

public record GetBudgetQuery(int Year, BudgetType Type);
