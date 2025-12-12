using Application.Schema.BudgetPlanning.Models;
using Application.Schema.BudgetTracking.Models;
using Application.Schema.Shared;
using Components.Shared;
using Components.Shared.Models;
using Microsoft.Extensions.Logging;
using GetTransactionCountQuery = Application.Schema.BudgetTracking.GetTransactionCount.Query;

namespace Components.BudgetTracking;

public partial class Tracker
{
    List<TransactionInfo> data = [];
    Month[] months = [];
    Month selectedMonth = Month.Jan;
    DateTime lastTransactionDate = DateTime.Today;
    int totalTransactions = 0;
    int totalTransactionsInCurrentYear = 0;
    decimal trackedBalance = 0.00m; // TODO fetch from backend

    string DaysSinceLastTransaction => DateTime.Today > lastTransactionDate
        ? $"({DateTime.Today.Subtract(lastTransactionDate).Days} days ago)"
        : string.Empty;
    string TotalTransactionsInCurrentYear => $"({totalTransactionsInCurrentYear} this year)";
    string BalanceAnalysis => trackedBalance >= 0
        ? "of tracked income left to be allocated"
        : "allocated not covered by income";

    public int CurrentMonth => (int)selectedMonth;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        AppStateManager.OnYearChangedAsync -= GetTransactionsAsync;
        AppStateManager.OnYearChangedAsync += GetTransactionsAsync;

        // initialize months
        months = Enum.GetValues<Month>();
        selectedMonth = State.Month;

        // initialize transaction counts
        await GetTransactionCountsAsync();

        // initialize last transaction date
        await GetLastTransactionDateAsync();

        // initialize transactions for the selected year
        await GetTransactionsAsync(State.Year);

        async Task GetTransactionCountsAsync()
        {
            var results = await Task.WhenAll(
                GetTransactionCountHandler.DoAsync(),
                GetTransactionCountHandler.DoAsync(new GetTransactionCountQuery(State.Year.ToString())))
                .ConfigureAwait(false);
            if (results[0].IsFailure || results[1].IsFailure)
            {
                if (Logger.IsEnabled(LogLevel.Error))
                {
                    var error = Error.Aggregate(results.Select(p => p.Error));
                    Logger.LogError("Failed to fetch transaction count(s): {Error}", error);
                }
                Notifier.Notify(Radzen.NotificationSeverity.Error, NotificationMessages.TransactionMetaDataFetchFailed);
                return;
            }
            totalTransactions = results[0].Value;
            totalTransactionsInCurrentYear = results[1].Value;
        }
    }

    async Task OnMonthChangedAsync() =>
        await Task.WhenAll(
            AppStateManager.SetMonthAsync(selectedMonth),
            GetTransactionsAsync(State.Year))
            .ConfigureAwait(false);

    async Task GetTransactionsAsync(int year)
    {
        var result = await GetTransactionHandler.DoAsync(new(year, CurrentMonth)).ConfigureAwait(false);
        if (result.IsFailure)
        {
            if (Logger.IsEnabled(LogLevel.Error))
                Logger.LogError("Failed to fetch transactions of {Year} and {Month}: {Error}", State.Year, DateTime.Now.Month, result.Error);
            Notifier.Notify(Radzen.NotificationSeverity.Error, NotificationMessages.TransactionFetchFailed);
            return;
        }
        data = result.Value;
    }

    async Task GetLastTransactionDateAsync()
    {
        var result = await GetLastTransactionDateHandler.DoAsync().ConfigureAwait(false);
        if (result.IsFailure)
        {
            if (Logger.IsEnabled(LogLevel.Error))
                Logger.LogError("Failed to fetch last transaction date: {Error}", result.Error);
            Notifier.Notify(Radzen.NotificationSeverity.Error, NotificationMessages.TransactionMetaDataFetchFailed);
            return;
        }
        lastTransactionDate = result.Value;
    }

    async Task OnTransactionChangeAsync(Tuple<DateTime, GridOperation> arg)
    {
        var (effectiveDate, operation) = arg;
        switch (operation)
        {
            case GridOperation.Create:
                lastTransactionDate = effectiveDate > lastTransactionDate ? effectiveDate : lastTransactionDate;
                totalTransactions += 1;
                totalTransactionsInCurrentYear = effectiveDate.Year == State.Year
                    ? totalTransactionsInCurrentYear + 1
                    : totalTransactionsInCurrentYear;
                break;
            case GridOperation.Update:
                if (effectiveDate < lastTransactionDate)
                    await GetLastTransactionDateAsync().ConfigureAwait(false);
                else
                    lastTransactionDate = effectiveDate;
                break;
            case GridOperation.Delete:
                if (effectiveDate == lastTransactionDate)
                    await GetLastTransactionDateAsync().ConfigureAwait(false);
                totalTransactions -= 1;
                totalTransactionsInCurrentYear = effectiveDate.Year == State.Year
                    ? totalTransactionsInCurrentYear - 1
                    : totalTransactionsInCurrentYear;
                break;
            default:
                if (Logger.IsEnabled(LogLevel.Warning))
                    Logger.LogWarning("Unhandled GridOperation: {Operation}", operation);
                break;
        }
    }
}