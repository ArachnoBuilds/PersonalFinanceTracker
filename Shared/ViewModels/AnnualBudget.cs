namespace ViewModels;

public record AnnualBudget(string Category, Dictionary<Header, double> Budget);