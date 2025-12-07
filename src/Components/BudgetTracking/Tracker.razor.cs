using Application.Schema.BudgetTracking.Models;
using Microsoft.Extensions.Logging;

namespace Components.BudgetTracking;

public partial class Tracker
{
    List<TransactionInfo> data = [];
    int CurrentMonth => DateTime.Now.Month;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        AppStateManager.OnYearChangedAsync -= PrepareTransactionsAsync;
        AppStateManager.OnYearChangedAsync += PrepareTransactionsAsync;

        // prepare transactions for the selected year
        await PrepareTransactionsAsync(State.Year);
    }
    
    async Task PrepareTransactionsAsync(int year)
    {
        var result = await GetTransactionHandler.DoAsync(new(year.ToString(), CurrentMonth.ToString()))
                        .ConfigureAwait(false);
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