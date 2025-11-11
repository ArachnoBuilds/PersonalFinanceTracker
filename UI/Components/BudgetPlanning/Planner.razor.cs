using Components.ViewModels;

namespace Components.BudgetPlanning;

public partial class Planner
{
    int[] years = [];
    int selectedYear;
    List<Budget> summaries = [];
    List<Budget> incomes = [];
    List<Budget> expenses = [];
    List<Budget> savings = [];
    List<string> incomeCategories = [];
    List<string> expenseCategories = [];
    List<string> savingCategories = [];

    protected override void OnInitialized()
    {
        years = [2025, 2026, 2027, 2028, 2029, 2030];
        selectedYear = 2025;

        // fetch budget data for the selected year
        incomes = Svc.GetIncomes(selectedYear).ToBudget();
        expenses = Svc.GetExpenses(selectedYear).ToBudget();
        savings = Svc.GetSavings(selectedYear).ToBudget();
        summaries = [
                Budget.ToSummaryBudget(
                    incomes.Find(p => p.IsTotalCategory) ?? Budget.EmptyTotal,
                    expenses.Find(p => p.IsTotalCategory) ?? Budget.EmptyTotal,
                    savings.Find(p => p.IsTotalCategory) ?? Budget.EmptyTotal)
            ];

        base.OnInitialized();
    }
}