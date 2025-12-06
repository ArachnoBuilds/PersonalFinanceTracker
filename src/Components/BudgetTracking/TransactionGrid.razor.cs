using Application.Shared;
using Application.Shared.Models;
using Components.BudgetTracking.Models;
using Components.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Radzen;
using Radzen.Blazor;

namespace Components.BudgetTracking;

public partial class TransactionGrid
{
    [Parameter]
    public List<Transaction> Data { get; set; } = [];

    RadzenDataGrid<Transaction>? grid;
    readonly BudgetType[] budgetTypes = [BudgetType.Income, BudgetType.Expenses, BudgetType.Savings];
    List<Category> categories = [];
    Dictionary<BudgetType, List<Category>> categoriesByType = [];
    int selectedCategoryId;
    GridOperation operation = GridOperation.None;

    protected async override Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        // initialize categories for the selected year
        List<Task<Result<List<Category>>>> getters =
        [
            GetCategoryHandler.DoAsync(new(BudgetType.Income, State.Year)),
            GetCategoryHandler.DoAsync(new(BudgetType.Expenses, State.Year)),
            GetCategoryHandler.DoAsync(new(BudgetType.Savings, State.Year))
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
                    Logger.LogError("Error fetching budget categories for year {Year}: {Error}",
                        State.Year,
                        p.Result.Error);
                });
            Notifier.Notify(NotificationSeverity.Error, NotificationMessages.CategoryFetchFailed);
            return;
        }
        categoriesByType = new()
        {
            [BudgetType.Income] = getters[0].Result.Value,
            [BudgetType.Expenses] = getters[1].Result.Value,
            [BudgetType.Savings] = getters[2].Result.Value
        };
    }

    async Task OnBudgetTypeChangedAsync(object arg)
    {
        if (arg is not BudgetType type)
            return;
        categories = categoriesByType.GetValueOrDefault(type, []);
    }

    void Reset()
    {
        operation = GridOperation.None;
        selectedCategoryId = -1;
    }

    async Task OnAddRowAsync()
    {
        if (grid == null || !grid.IsValid || operation is not GridOperation.None)
        {
            Notifier.Notify(NotificationSeverity.Error, NotificationMessages.TransactionCreateInitializationFailed);
            return;
        }

        operation = GridOperation.Create;
        selectedCategoryId = -1;
        await grid.InsertRow(new()
        {
            Date = DateTime.Now
        });
    }

    async Task OnEditRowAsync(Transaction transaction)
    {
        // TODO notification should be more specific
        if (grid == null || !grid.IsValid || operation is not GridOperation.None)
        {
            Notifier.Notify(NotificationSeverity.Error, NotificationMessages.TransactionUpdateInitializationFailed);
            return;
        }

        operation = GridOperation.Update;
        selectedCategoryId = transaction.BudgetCategory.Id;
        await grid.EditRow(transaction);
    }

    async Task OnSaveRowAsync(Transaction transaction)
    {
        // TODO add notification 
        if (grid == null || !grid.IsValid)
            return;

        // save changes
        await grid.UpdateRow(transaction);
        Reset();
    }

    async Task OnCancelEditAsync(Transaction transaction)
    {
        if (grid == null)
            return;

        grid.CancelEditRow(transaction);
        Reset();
    }

    async Task OnCreateAsync(Transaction transaction)
    {
        if (operation is not GridOperation.Create)
            return;

        Data.Add(transaction);
        Notifier.Notify(NotificationSeverity.Success, NotificationMessages.TransactionCreationSuccess);
        Reset();
    }

    async Task OnUpdateAsync(Transaction transaction)
    {
        if (operation is not GridOperation.Update)
            return;

        Data.RemoveAll(p => p.Id == transaction.Id);
        Data.Add(transaction);
        Notifier.Notify(NotificationSeverity.Success, NotificationMessages.TransactionUpdationSuccess);
        Reset();
    }

    async Task OnDeleteAsync(Transaction transaction)
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

        Data.Remove(transaction);
        Notifier.Notify(NotificationSeverity.Success, NotificationMessages.TransactionDeletionSuccess);

        await grid.Reload();
    }
}