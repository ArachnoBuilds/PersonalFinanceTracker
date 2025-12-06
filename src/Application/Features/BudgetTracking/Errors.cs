using Application.Schema.Shared;

namespace Application.Features.BudgetTracking;

internal static class Errors
{
    internal static Error BudgetTypeSummaryNotAllowed =>
        new("BudgetTypeSummaryNotAllowed",
            "The budget type 'Summary' is not allowed for this operation.");
    internal static Error BudgetCategoryNotFound =>
        new("BudgetCategoryNotFound",
            "The specified budget category was not found.");
}
