using Application.Shared.Models;

namespace Application.Features.BudgetPlanning.GetBudget;

public record GetBudgetQuery(int Year, BudgetType Type);
