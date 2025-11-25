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
    int selectedCategoryId;
    GridOperation operation = GridOperation.None;

    protected async override Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }

    async Task OnBudgetTypeChangedAsync(object arg)
    {
        if (arg is not BudgetType type)
            return;
        var getterResult = await GetCategoryHandler.DoAsync(new(type, State.Year));
        if (getterResult.IsFailure)
        {
            Logger.LogError("Failed to get category descriptions for budget type {Type}: {Error}", type, getterResult.Error);
            Notifier.Notify(NotificationSeverity.Error, NotificationMessages.CategoryFetchFailed);
            return;
        }
        categories = getterResult.Value;
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