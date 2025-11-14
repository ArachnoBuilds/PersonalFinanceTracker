using Application.Features.BudgetPlanning.Models;
using Application.Shared.Models;

namespace Application.Features.BudgetPlanning.CreateBudget;

public record CreateBudgetCommand(int Year, BudgetType Type, AnnualBudget Budget);
