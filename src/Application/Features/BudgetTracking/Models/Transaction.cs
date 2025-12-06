namespace Application.Features.BudgetTracking.Models;

public record Transaction(
    string Id,
    DateTime Date,
    BudgetInfo BudgetInfo,
    string Account,
    decimal Amount,
    string Description,
    DateTime EffectiveDate);