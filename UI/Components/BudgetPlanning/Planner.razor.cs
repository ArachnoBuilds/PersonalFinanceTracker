using ViewModels;

namespace Components.BudgetPlanning;

public partial class Planner
{
    int[] years = [];
    int selectedYear;
    Dictionary<Header, MonthlyBudgetSummary> summaries = [];
    List<AnnualBudget> incomeGrid = [];
    List<AnnualBudget> expensesGrid = [];
    List<AnnualBudget> savingsGrid = [];

    protected override void OnInitialized()
    {
        years = [2025, 2026, 2027, 2028, 2029, 2030];
        selectedYear = 2025;
        base.OnInitialized();
    }
}