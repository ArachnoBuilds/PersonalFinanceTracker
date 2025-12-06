using Application.Schema.BudgetPlanning.Models;
using Application.Schema.Shared.Models;
using Components.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Radzen;
using Radzen.Blazor;

namespace Components.BudgetPlanning;

public partial class BudgetGrid
{
    [Parameter]
    public BudgetItemType Type { get; set; } = BudgetItemType.Summary;
    [Parameter]
    public List<BudgetInfo> Data { get; set; } = [];
    [Parameter]
    public EventCallback<List<BudgetInfo>> DataChanged { get; set; }
    [Parameter]
    public EventCallback RecalculateBudgetSummary { get; set; }

    RadzenDataGrid<BudgetInfo>? grid;
    List<BudgetItem> budgetItems = [];
    string FirstColumnHeader => Type switch
    {
        BudgetItemType.Summary => string.Empty,
        _ => Type.ToString()
    };
    GridOperation operation = GridOperation.None;
    string selectedBudgetItemDesc = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if (Type is BudgetItemType.Summary)
            return;

        var getterResult = await GetBudgetItemHandler.DoAsync(new(Type));
        if (getterResult.IsFailure)
        {
            Logger.LogError("Failed to get category descriptions for budget type {Type}: {Error}", Type, getterResult.Error);
            return;
        }
        budgetItems = getterResult.Value;
    }

    void Reset()
    {
        operation = GridOperation.None;
        selectedBudgetItemDesc = string.Empty;
    }

    async Task OnAddRowAsync()
    {
        if (grid == null || !grid.IsValid || operation is not GridOperation.None)
        {
            Notifier.Notify(NotificationSeverity.Error, NotificationMessages.BudgetCreateInitializationFailed);
            return;
        }

        operation = GridOperation.Create;
        selectedBudgetItemDesc = string.Empty;
        await grid.InsertRow(new()
        {
            BudgetItemId = -1
        });
    }

    async Task OnEditRowAsync(BudgetInfo budget)
    {
        if (grid == null || !grid.IsValid || operation is not GridOperation.None)
        {
            Notifier.Notify(NotificationSeverity.Error, NotificationMessages.BudgetUpdateInitializationFailed);
            return;
        }

        operation = GridOperation.Update;
        selectedBudgetItemDesc = budget.BudgetItemDesc;
        await grid.EditRow(budget);
    }

    async Task OnSaveRowAsync(BudgetInfo budget)
    {
        if (grid == null || !grid.IsValid)
            return;

        // set category details based on operation
        switch (operation)
        {
            case GridOperation.Create:
                {
                    // validate
                    if (Data.Exists(p => p.BudgetItemDesc == selectedBudgetItemDesc))
                    {
                        Notifier.Notify(NotificationSeverity.Error, NotificationMessages.BudgetCategoryAlreadyExists);
                        return;
                    }

                    // update
                    var selectedCategory = budgetItems.Find(p => p.Description == selectedBudgetItemDesc);
                    if (selectedCategory == null)
                    {
                        budget.BudgetItemId = -1;
                        budget.BudgetItemDesc = selectedBudgetItemDesc;
                    }
                    else
                    {
                        budget.BudgetItemId = selectedCategory.Id;
                        budget.BudgetItemDesc = selectedCategory.Description;
                    }
                    break;
                }
            case GridOperation.Update:
                {
                    // validate
                    if (Data.Exists(p => p.BudgetItemId != budget.BudgetItemId && p.BudgetItemDesc == selectedBudgetItemDesc))
                    {
                        Notifier.Notify(NotificationSeverity.Error, NotificationMessages.BudgetCategoryAlreadyExists);
                        return;
                    }

                    // update
                    budget.BudgetItemDesc = selectedBudgetItemDesc;
                    break;
                }
            default:
                {
                    if (Logger.IsEnabled(LogLevel.Error))
                        Logger.LogError("Invalid operation {Operation} in OnSaveRowAsync", operation);
                    return;
                }
        }

        // save changes
        await grid.UpdateRow(budget);
        Reset();
    }

    async Task OnCancelEditAsync(BudgetInfo budget)
    {
        if (grid == null)
            return;

        // cancel changes
        grid.CancelEditRow(budget);
        Reset();
    }

    async Task OnCreateAsync(BudgetInfo budget)
    {
        if (operation is not GridOperation.Create)
            return;

        var annualBudget = budget.ToBudget();
        var result = await CreateBudgetHandler.DoAsync(new(State.Year, Type, annualBudget)).ConfigureAwait(false);
        if (result.IsFailure)
        {
            Logger.LogError("Failed to create budget: {Error}", result.Error);
            Notifier.Notify(NotificationSeverity.Error, NotificationMessages.BudgetCreationFailed);
        }
        else
        {
            if (budget.BudgetItemId == -1)
                budget.BudgetItemId = result.Value;
            Data.Add(budget);
            BudgetInfo.RecalculateTotal(Data);
            await RecalculateBudgetSummary.InvokeAsync();
            Notifier.Notify(NotificationSeverity.Success, NotificationMessages.BudgetCreationSuccess);
        }
        Reset();
    }

    async Task OnUpdateAsync(BudgetInfo budget)
    {
        if (operation is not GridOperation.Update)
            return;

        var annualBudget = budget.ToBudget();
        var result = await UpdateBudgetHandler.DoAsync(new(State.Year, annualBudget)).ConfigureAwait(false);
        if (result.IsFailure)
        {
            Logger.LogError("Failed to update budget: {Error}", result.Error);
            Notifier.Notify(NotificationSeverity.Error, NotificationMessages.BudgetUpdationFailed);
            return;
        }
        else
        {
            Data.RemoveAll(p => p.BudgetItemId == budget.BudgetItemId);
            Data.Add(budget);
            BudgetInfo.RecalculateTotal(Data);
            await RecalculateBudgetSummary.InvokeAsync();
            Notifier.Notify(NotificationSeverity.Success, NotificationMessages.BudgetUpdationSuccess);
        }
        Reset();
    }

    async Task OnDeleteAsync(BudgetInfo budget)
    {
        if (grid == null)
            return;

        var confirm = await DlgSvc.Confirm(
            "Are you sure?",
            $"Delete budget for {budget.BudgetItemDesc}",
            new ConfirmOptions() { OkButtonText = "Yes", CancelButtonText = "No" })
            .ConfigureAwait(false);
        if (confirm.HasValue && !confirm.Value)
            return;

        var result = await DeleteBudgetHandler.DoAsync(new(State.Year, budget.BudgetItemId)).ConfigureAwait(false);
        if (result.IsFailure)
        {
            Logger.LogError("Failed to delete budget: {Error}", result.Error);
            Notifier.Notify(NotificationSeverity.Error, NotificationMessages.BudgetDeletionFailed);
            return;
        }
        else
        {
            Data.Remove(budget);
            BudgetInfo.RecalculateTotal(Data);
            // TODO needed to be fixed
            await RecalculateBudgetSummary.InvokeAsync().ConfigureAwait(false);
            Notifier.Notify(NotificationSeverity.Success, NotificationMessages.BudgetDeletionSuccess);
        }

        await grid.Reload();
    }
}
