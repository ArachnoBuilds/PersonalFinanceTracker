using Application.Shared.Models;

namespace Application.Features.BudgetTracking.GetCategory;

public record GetCategoryQuery(BudgetType Type, int Year);
