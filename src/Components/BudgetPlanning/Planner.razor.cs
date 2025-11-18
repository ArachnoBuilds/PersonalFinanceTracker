using Application.Features.BudgetPlanning.Models;
using Application.Shared;
using Application.Shared.Models;
using Microsoft.Extensions.Logging;

namespace Components.BudgetPlanning;

using AnnualBudgetsResult = Result<List<AnnualBudget>>;

public partial class Planner
{
    List<Models.Budget> summaries = [];
    List<Models.Budget> incomes = [];
    List<Models.Budget> expenses = [];
    List<Models.Budget> savings = [];

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

        StateHasChanged();
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