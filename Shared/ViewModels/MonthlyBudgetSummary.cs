namespace ViewModels;

public record MonthlyBudgetSummary(
    double TotalIncome,
    double TotalExpenses,
    double TotalSavings)
{
    public double Balance => TotalIncome - (TotalExpenses + TotalSavings);
}