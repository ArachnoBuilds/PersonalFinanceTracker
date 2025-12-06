namespace Application.Schema.BudgetTracking.Models;

public record Transaction(
    string Id,
    DateTime Date,
    string BudgetId,
    string Account,
    decimal Amount,
    string Description,
    DateTime EffectiveDate);