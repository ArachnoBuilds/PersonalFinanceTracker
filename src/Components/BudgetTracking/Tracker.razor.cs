using Application.Schema.BudgetPlanning.Models;
using Application.Schema.BudgetTracking.Models;
using Microsoft.Extensions.Logging;

namespace Components.BudgetTracking;

public partial class Tracker
{
    List<TransactionInfo> data = [];
    Month[] months = [];
    Month selectedMonth = Month.Jan;

    public int CurrentMonth => (int)selectedMonth;


    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        AppStateManager.OnYearChangedAsync -= PrepareTransactionsAsync;
        AppStateManager.OnYearChangedAsync += PrepareTransactionsAsync;

        // initialize months
        months = Enum.GetValues<Month>();
        selectedMonth = State.Month;

        // prepare transactions for the selected year
        await PrepareTransactionsAsync(State.Year);
    }

    async Task OnMonthChangedAsync() => 
        await Task.WhenAll(
            AppStateManager.SetMonthAsync(selectedMonth),
            PrepareTransactionsAsync(State.Year))
            .ConfigureAwait(false);

    async Task PrepareTransactionsAsync(int year)
    {
        var result = await GetTransactionHandler.DoAsync(new(year.ToString(), CurrentMonth.ToString())).ConfigureAwait(false);
        if (result.IsFailure)
        {
            if (Logger.IsEnabled(LogLevel.Error))
                Logger.LogError("Failed to fetch transactions of {Year} and {Month}: {Error}", State.Year, DateTime.Now.Month, result.Error);
            Notifier.Notify(Radzen.NotificationSeverity.Error, NotificationMessages.TransactionFetchFailed);
            return;
        }
        data = result.Value;
    }
}