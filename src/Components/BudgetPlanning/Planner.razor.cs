using Application.Features.BudgetPlanning.Models;
using Application.Shared;
using Application.Shared.Models;
using Microsoft.Extensions.Logging;
using Radzen;

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

    protected override async Task OnInitializedAsync()
    {
        years = [2025, 2026, 2027, 2028, 2029, 2030];
        selectedYear = 2025;

        // prepare budgets for the selected year
        await PrepareBudgetsAsync();

        await base.OnInitializedAsync();
    }

    async Task PrepareBudgetsAsync() 
    {
        // fetch budgets for the selected year
        List<Task<AnnualBudgetsResult>> getters = [
            GetBudgetHandler.DoAsync(new(selectedYear, BudgetType.Income)),
            GetBudgetHandler.DoAsync(new(selectedYear, BudgetType.Expenses)),
            GetBudgetHandler.DoAsync(new(selectedYear, BudgetType.Savings))
        ];
        await Task.WhenAll(getters);

        // check for errors
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
            Notifier.Notify(Radzen.NotificationSeverity.Error, NotificationMessages.BudgetFetchFailed);
            return;
        }

        // map then set budgets
        incomes = getters[0].Result.Value.ToBudget();
        expenses = getters[1].Result.Value.ToBudget();
        savings = getters[2].Result.Value.ToBudget();

        // calculate summary
        RecalculateSummary();
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