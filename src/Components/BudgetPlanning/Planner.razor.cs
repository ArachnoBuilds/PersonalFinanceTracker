using Application.Schema.BudgetPlanning.Models;
using Application.Schema.Shared;
using Application.Schema.Shared.Models;
using Microsoft.Extensions.Logging;

namespace Components.BudgetPlanning;

using AnnualBudgetsResult = Result<List<Budget>>;

public partial class Planner
{
    List<BudgetInfo> summaries = [];
    List<BudgetInfo> incomes = [];
    List<BudgetInfo> expenses = [];
    List<BudgetInfo> savings = [];

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        AppStateManager.OnYearChangedAsync -= PrepareBudgetsAsync;
        AppStateManager.OnYearChangedAsync += PrepareBudgetsAsync;

        // prepare budgets for the selected year
        await PrepareBudgetsAsync(State.Year);
    }

    async Task PrepareBudgetsAsync(int selectedYear) 
    {
        // fetch budgets for the selected year
        List<Task<Result<List<BudgetInfo>>>> getters = [
            GetBudgetHandler.DoAsync(new(selectedYear, BudgetItemType.Income)),
            GetBudgetHandler.DoAsync(new(selectedYear, BudgetItemType.Expenses)),
            GetBudgetHandler.DoAsync(new(selectedYear, BudgetItemType.Savings))
        ];
        await Task.WhenAll(getters);

        // check for errors
        if (getters.Exists(p => p.Result.IsFailure))
        {
            getters
                .FindAll(p => p.Result.IsFailure)
                .ForEach(p =>
                {
                    if (!Logger.IsEnabled(LogLevel.Error))
                        return;
                    Logger.LogError("Error fetching budgets for year {Year}: {Error}",
                        selectedYear,
                        p.Result.Error);
                });
            Notifier.Notify(Radzen.NotificationSeverity.Error, NotificationMessages.BudgetFetchFailed);
            return;
        }

        // set budgets
        incomes = getters[0].Result.Value;
        expenses = getters[1].Result.Value;
        savings = getters[2].Result.Value;

        // calculate summary
        RecalculateSummary();

        StateHasChanged();
    }
    void RecalculateSummary()
    {
        summaries =
        [
            BudgetInfo.ToSummaryBudget(
                incomes.Find(p => p.IsTotalCategory) ?? BudgetInfo.EmptyTotal,
                expenses.Find(p => p.IsTotalCategory) ??  BudgetInfo.EmptyTotal,
                savings.Find(p => p.IsTotalCategory) ?? BudgetInfo.EmptyTotal)
        ];
    }
}