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

        // fetch budget data for the selected year
        incomeGrid = Svc.GetIncomes(selectedYear);
        expensesGrid = Svc.GetExpenses(selectedYear);
        savingsGrid = Svc.GetSavings(selectedYear);

        // calculate summaries
        summaries = MonthlyBudgetSummary.Calculate(incomeGrid, expensesGrid, savingsGrid);

        base.OnInitialized();
    }
}