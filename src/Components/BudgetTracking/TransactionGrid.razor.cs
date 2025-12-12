using Components.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Radzen;
using Radzen.Blazor;
using Application.Schema.Shared;
using Application.Schema.Shared.Models;
using Application.Schema.BudgetTracking.Models;

namespace Components.BudgetTracking;

public partial class TransactionGrid
{
    [Parameter]
    public List<TransactionInfo> Data { get; set; } = [];
    [Parameter]
    public int CurrentMonth { get; set; } = DateTime.Now.Month;
    [Parameter]
    public EventCallback<Tuple<DateTime, GridOperation>> OnTransactionChange { get; set; }

    RadzenDataGrid<TransactionInfo>? grid;
    readonly BudgetItemType[] budgetItemTypes = [BudgetItemType.Income, BudgetItemType.Expenses, BudgetItemType.Savings];
    readonly List<Budget> budgets = [];
    readonly List<string> accounts = [];
    Dictionary<BudgetItemType, List<Budget>> budgetsByType = [];
    string selectedBudgetId = string.Empty;
    string selectedAccount = string.Empty;
    GridOperation operation = GridOperation.None;

    protected async override Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await Task.WhenAll(
            InitializeBudgetsAsync(),
            InitializeAccountsAsync())
            .ConfigureAwait(false);

        async Task InitializeBudgetsAsync()
        {
            List<Task<Result<List<Budget>>>> getters =
            [
                GetBudgetHandler.DoAsync(new(BudgetItemType.Income, State.Year, CurrentMonth)),
                GetBudgetHandler.DoAsync(new(BudgetItemType.Expenses, State.Year, CurrentMonth)),
                GetBudgetHandler.DoAsync(new(BudgetItemType.Savings, State.Year, CurrentMonth)),
            ];
            await Task.WhenAll(getters).ConfigureAwait(false);
            if (getters.Exists(p => p.Result.IsFailure))
            {
                getters
                    .FindAll(p => p.Result.IsFailure)
                    .ForEach(p =>
                    {
                        if (!Logger.IsEnabled(LogLevel.Error))
                            return;
                        Logger.LogError("Error fetching budgets for year {Year}: {Error}",
                            State.Year,
                            p.Result.Error);
                    });
                Notifier.Notify(NotificationSeverity.Error, NotificationMessages.BudgetFetchFailed);
                return;
            }
            budgetsByType = new()
            {
                [BudgetItemType.Income] = getters[0].Result.Value,
                [BudgetItemType.Expenses] = getters[1].Result.Value,
                [BudgetItemType.Savings] = getters[2].Result.Value
            };
        }
        async Task InitializeAccountsAsync()
        {
            var r = await GetAccountHandler.DoAsync().ConfigureAwait(false);
            if (r.IsFailure)
            {
                if (Logger.IsEnabled(LogLevel.Error))
                    Logger.LogError("Error fetching accounts: {Error}", r.Error);
                Notifier.Notify(NotificationSeverity.Error, NotificationMessages.AccountFetchFailed);
                return;
            }
            accounts.AddRange(r.Value);
        }
    }

    void SetBudgetOptions(BudgetItemType type)
    {
        var values = budgetsByType.GetValueOrDefault(type, []);
        budgets.Clear();
        budgets.AddRange(values);
    }

    async Task OnBudgetItemTypeChangedAsync(object arg)
    {
        if (arg is not BudgetItemType type)
            return;
        SetBudgetOptions(type);
    }

    void Reset()
    {
        operation = GridOperation.None;
        selectedBudgetId = string.Empty;
        selectedAccount = string.Empty;
    }

    void LoadData()
    {
        var sortedData = Data.OrderByDescending(p => p.EffectiveDate).ToList();
        Data.Clear();
        Data.AddRange(sortedData);
    }

    async Task OnAddRowAsync()
    {
        if (grid == null || !grid.IsValid || operation is not GridOperation.None)
        {
            Notifier.Notify(NotificationSeverity.Error, NotificationMessages.TransactionCreateInitializationFailed);
            return;
        }

        operation = GridOperation.Create;
        TransactionInfo transaction = new()
        {
            Date = DateTime.Now
        };
        SetBudgetOptions(transaction.BudgetType);
        selectedBudgetId = string.Empty;
        selectedAccount = string.Empty;
        await grid.InsertRow(transaction);
    }

    async Task OnEditRowAsync(TransactionInfo transaction)
    {
        // TODO notification should be more specific
        if (grid == null || !grid.IsValid || operation is not GridOperation.None)
        {
            Notifier.Notify(NotificationSeverity.Error, NotificationMessages.TransactionUpdateInitializationFailed);
            return;
        }

        operation = GridOperation.Update;
        SetBudgetOptions(transaction.BudgetType);
        selectedBudgetId = transaction.Budget.Id;
        selectedAccount = transaction.Account;
        await grid.EditRow(transaction);
    }

    async Task OnSaveRowAsync(TransactionInfo transaction)
    {
        // TODO add notification 
        if (grid == null || !grid.IsValid)
            return;

        // set category
        var selectedBudget = budgets.FirstOrDefault(c => c.Id == selectedBudgetId);
        if (selectedBudget == null)
        {
            Notifier.Notify(NotificationSeverity.Error, NotificationMessages.BudgetNotSelected);
            return;
        }
        else
            transaction.Budget = selectedBudget;

        // set account
        transaction.Account = selectedAccount;

        // set effective date
        transaction.EffectiveDate = transaction.BudgetType is BudgetItemType.Income &&
                                    State.ShiftLateIncomeStatus &&
                                    transaction.Date.Day >= State.ShiftLateIncomeStartingDay
                                    ? new DateTime(transaction.Date.Year, transaction.Date.Month, 1).AddMonths(1)
                                    : transaction.Date;

        // save changes
        await grid.UpdateRow(transaction);
        await grid.Reload();
        Reset();
    }

    async Task OnCancelEditAsync(TransactionInfo transaction)
    {
        if (grid == null)
            return;

        grid.CancelEditRow(transaction);
        Reset();
    }

    async Task OnCreateAsync(TransactionInfo transaction)
    {
        if (operation is not GridOperation.Create)
            return;

        var creatable = transaction.ToTransaction();
        var result = await CreateTransactionHandler.DoAsync(new(creatable)).ConfigureAwait(false);
        if (result.IsFailure)
        {
            if (Logger.IsEnabled(LogLevel.Error))
                Logger.LogError("Error creating transaction: {Error}", result.Error);
            Notifier.Notify(NotificationSeverity.Error, NotificationMessages.TransactionCreationFailed);
        }
        else
        {
            if (transaction.EffectiveDate.Month == CurrentMonth)
                Data.Add(transaction);
            else
                Notifier.Notify(NotificationSeverity.Info, $"Please change the month to {transaction.EffectiveDate:MMMM} to view the transaction.", duration: 5000);
            Notifier.Notify(NotificationSeverity.Success, NotificationMessages.TransactionCreationSuccess);
            await OnTransactionChange.InvokeAsync(new(transaction.EffectiveDate, GridOperation.Create));
        }
        Reset();
    }

    async Task OnUpdateAsync(TransactionInfo transaction)
    {
        if (operation is not GridOperation.Update)
            return;
        var updatable = transaction.ToTransaction();
        var result = await UpdateTransactionHandler.DoAsync(new(updatable)).ConfigureAwait(false);
        if (result.IsFailure)
        {
            if (Logger.IsEnabled(LogLevel.Error))
                Logger.LogError("Error updating transaction: {Error}", result.Error);
            Notifier.Notify(NotificationSeverity.Error, NotificationMessages.TransactionUpdationFailed);
        }
        else
        {
            Data.RemoveAll(p => p.Id == transaction.Id);
            if (transaction.EffectiveDate.Month == CurrentMonth)
                Data.Add(transaction);
            else
                Notifier.Notify(NotificationSeverity.Info, $"Please change the month to {transaction.EffectiveDate:MMMM} to view the transaction.", duration: 5000);
            Notifier.Notify(NotificationSeverity.Success, NotificationMessages.TransactionUpdationSuccess);
            await OnTransactionChange.InvokeAsync(new(transaction.EffectiveDate, GridOperation.Update));
        }
        Reset();
    }

    async Task OnDeleteAsync(TransactionInfo transaction)
    {
        if (grid == null)
            return;

        var confirm = await DlgSvc.Confirm(
            "Are you sure?",
            $"Delete transaction",
            new ConfirmOptions() { OkButtonText = "Yes", CancelButtonText = "No" })
            .ConfigureAwait(false);
        if (confirm.HasValue && !confirm.Value)
            return;

        var result = await DeleteTransactionHandler.DoAsync(new(transaction.Id)).ConfigureAwait(false);
        if (result.IsFailure)
        {
            if (Logger.IsEnabled(LogLevel.Error))
                Logger.LogError("Error deleting transaction: {Error}", result.Error);
            Notifier.Notify(NotificationSeverity.Error, NotificationMessages.TransactionDeletionFailed);
        }
        else
        {
            Data.Remove(transaction);
            Notifier.Notify(NotificationSeverity.Success, NotificationMessages.TransactionDeletionSuccess);
            await OnTransactionChange.InvokeAsync(new(transaction.EffectiveDate, GridOperation.Delete));
        }
        await grid.Reload();
    }
}