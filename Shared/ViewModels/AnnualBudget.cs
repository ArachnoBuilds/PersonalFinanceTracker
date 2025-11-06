namespace ViewModels;

public record AnnualBudget(string Category, Dictionary<Month, double> Budget);