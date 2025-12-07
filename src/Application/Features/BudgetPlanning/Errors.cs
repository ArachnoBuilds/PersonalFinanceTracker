using Application.Schema.Shared;

namespace Application.Features.BudgetPlanning;

internal static class Errors
{
    internal static Error BudgetTypeSummaryNotAllowed =>
        new("BudgetTypeSummaryNotAllowed",
            "The budget type 'Summary' is not allowed for this operation.");
    internal static Error BudgetItemNotFound =>
        new("BudgetCategoryNotFound",
            "The specified budget category was not found.");
}
