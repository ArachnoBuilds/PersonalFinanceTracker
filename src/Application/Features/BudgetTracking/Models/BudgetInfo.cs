using Application.Shared.Models;

namespace Application.Features.BudgetTracking.Models;

public record BudgetInfo(string Id, BudgetType Type, Category Category);
