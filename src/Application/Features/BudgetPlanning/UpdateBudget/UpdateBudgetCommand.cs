using Application.Features.BudgetPlanning.Models;

namespace Application.Features.BudgetPlanning.UpdateBudget;

public record UpdateBudgetCommand(int Year, AnnualBudget Budget);