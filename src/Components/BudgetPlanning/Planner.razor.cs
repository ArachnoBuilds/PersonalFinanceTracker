using Application.Features.BudgetPlanning.Models;
using Application.Shared;
using Application.Shared.Models;
using Microsoft.Extensions.Logging;

namespace Components.BudgetPlanning;

using AnnualBudgetsResult = Result<List<AnnualBudget>>;

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

    protected override async Task OnInitializedAsync()
    {
        years = [2025, 2026, 2027, 2028, 2029, 2030];
        selectedYear = 2025;

        // fetch budgets for the selected year
        List<Task<AnnualBudgetsResult>> getters = [
            GetBudgetHandler.DoAsync(new(selectedYear, BudgetType.Income)),
            GetBudgetHandler.DoAsync(new(selectedYear, BudgetType.Expenses)),
            GetBudgetHandler.DoAsync(new(selectedYear, BudgetType.Savings))
        ];
        await Task.WhenAll(getters);
        if (getters.Exists(p => p.Result.IsFailure))
        {
            getters
                .FindAll(p => p.Result.IsFailure)
                .ForEach(p =>
                {
                    // TODO check why Logger.IsEnabled is not behaving as expectation
                    //if (!Logger.IsEnabled(LogLevel.Error))
                    //    return;
                    Logger.LogError("Error fetching budgets for year {Year}: {Error}",
                        selectedYear,
                        p.Result.Error);
                });
            return;
        }

        incomes = getters[0].Result.Value.ToBudget();
        expenses = getters[1].Result.Value.ToBudget();
        savings = getters[2].Result.Value.ToBudget();
        RecalculateSummary();

        await base.OnInitializedAsync();
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