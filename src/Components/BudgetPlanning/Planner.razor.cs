namespace Components.BudgetPlanning;

public partial class Planner
{
    int[] years = [];
    int selectedYear;
    List<Models.Budget> summaries = [];
    List<Models.Budget> incomes = [];
    List<Models.Budget> expenses = [];
    List<Models.Budget> savings = [];
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
        RecalculateSummary();

        base.OnInitialized();
    }

    void RecalculateSummary()
    {
        summaries = 
        [
            Models.Budget.ToSummaryBudget(
                incomes.Find(p => p.IsTotalCategory) ?? Models.Budget.EmptyTotal,
                expenses.Find(p => p.IsTotalCategory) ??  Models.Budget.EmptyTotal,
                savings.Find(p => p.IsTotalCategory) ?? Models.Budget.EmptyTotal)
        ];
    }
}