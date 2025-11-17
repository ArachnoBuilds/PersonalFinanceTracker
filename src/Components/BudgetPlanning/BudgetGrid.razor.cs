using Application.Features.BudgetPlanning.Models;
using Application.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Radzen;
using Radzen.Blazor;

namespace Components.BudgetPlanning;

public partial class BudgetGrid
{
    [Parameter]
    public int Year { get; set; }
    [Parameter]
    public BudgetType Type { get; set; } = BudgetType.Summary;
    [Parameter]
    public List<Models.Budget> Data { get; set; } = [];
    [Parameter]
    public EventCallback<List<Models.Budget>> DataChanged { get; set; }
    [Parameter]
    public EventCallback RecalculateBudgetSummary { get; set; }

    RadzenDataGrid<Models.Budget>? grid;
    Models.Budget? editable;
    List<Category> categories = [];
    string FirstColumnHeader => Type switch
    {
        BudgetType.Summary => string.Empty,
        _ => Type.ToString()
    };
    Operation operation = Operation.None;
    string selectedCategoryDesc = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        if (Type is not BudgetType.Summary)
        {
            var getterResult = await GetCategoryDescriptionHandler.DoAsync(new(Type));
            if (getterResult.IsFailure)
            {
                Logger.LogError("Failed to get category descriptions for budget type {Type}: {Error}", Type, getterResult.Error);
                return;
            }
            categories = getterResult.Value;
        }
        // TODO check later if this is needed
        //categories.RemoveAll(p => Data.Exists(d => d.CategoryDesc == p.Description));
        await base.OnInitializedAsync();
    }

    async Task OnAddRowAsync()
    {
        // TODO notification should be more specific
        if (grid == null || !grid.IsValid || operation is not Operation.None)
        {
            Notifier.Notify(NotificationSeverity.Error, NotificationMessages.BudgetCreationFailed);
            return;
        }

        operation = Operation.Create;
        selectedCategoryDesc = string.Empty;
        await grid.InsertRow(new()
        {
            CategoryId = -1
        });
    }

    async Task OnEditRowAsync(Models.Budget budget)
    {
        // TODO notification should be more specific
        if (grid == null || !grid.IsValid || operation is not Operation.None)
        {
            Notifier.Notify(NotificationSeverity.Error, NotificationMessages.BudgetUpdationFailed);
            return;
        }

        operation = Operation.Update;
        selectedCategoryDesc = budget.CategoryDesc;
        await grid.EditRow(budget);
    }

    async Task OnSaveRowAsync(Models.Budget budget)
    {
        // TODO add notification 
        if (grid == null || !grid.IsValid)
            return;

        // set category details based on operation
        switch (operation)
        {
            case Operation.Create:
                {
                    // validate
                    if (Data.Exists(p => p.CategoryDesc == selectedCategoryDesc))
                    {
                        Notifier.Notify(NotificationSeverity.Error, NotificationMessages.BudgetCategoryAlreadyExists);
                        return;
                    }

                    // update
                    var selectedCategory = categories.Find(p => p.Description == selectedCategoryDesc);
                    if (selectedCategory == null)
                    {
                        budget.CategoryId = -1;
                        budget.CategoryDesc = selectedCategoryDesc;
                    }
                    else
                    {
                        budget.CategoryId = selectedCategory.Id;
                        budget.CategoryDesc = selectedCategory.Description;
                    }
                    break;
                }
            case Operation.Update:
                {
                    // validate
                    if (Data.Exists(p => p.CategoryId != budget.CategoryId && p.CategoryDesc == selectedCategoryDesc))
                    {
                        Notifier.Notify(NotificationSeverity.Error, NotificationMessages.BudgetCategoryAlreadyExists);
                        return;
                    }

                    // update
                    budget.CategoryDesc = selectedCategoryDesc;
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

        // reset
        selectedCategoryDesc = string.Empty;
        operation = Operation.None;
    }

    async Task OnCancelEditAsync(Models.Budget budget)
    {
        if (grid == null)
            return;

        grid.CancelEditRow(budget);

        if (editable == null)
            return;
        budget = editable!;
        editable = null;
    }

    async Task OnCreateAsync(Models.Budget budget)
    {
        if (operation is not Operation.Create)
            return;

        var annualBudget = budget.ToAnnualBudget();
        var result = await CreateBudgetHandler.DoAsync(new(Year, Type, annualBudget)).ConfigureAwait(false);
        if (result.IsFailure)
        {
            Logger.LogError("Failed to create budget: {Error}", result.Error);
            Notifier.Notify(NotificationSeverity.Error, NotificationMessages.BudgetCreationFailed);
        }
        else
        {
            if (budget.CategoryId == -1)
                budget.CategoryId = result.Value;
            Data.Add(budget);
            Models.Budget.RecalculateTotal(Data);
            await RecalculateBudgetSummary.InvokeAsync();
            Notifier.Notify(NotificationSeverity.Success, NotificationMessages.BudgetCreationSuccess);
        }

    }

    async Task OnUpdateAsync(Models.Budget budget)
    {
        if (operation is not Operation.Update)
            return;

        var annualBudget = budget.ToAnnualBudget();
        var result = await UpdateBudgetHandler.DoAsync(new(Year, annualBudget)).ConfigureAwait(false);
        if (result.IsFailure)
        {
            Logger.LogError("Failed to update budget: {Error}", result.Error);
            Notifier.Notify(NotificationSeverity.Error, NotificationMessages.BudgetUpdationFailed);
            return;
        }
        else
        {
            Data.RemoveAll(p => p.CategoryId == budget.CategoryId);
            Data.Add(budget);
            Models.Budget.RecalculateTotal(Data);
            await RecalculateBudgetSummary.InvokeAsync();
            Notifier.Notify(NotificationSeverity.Success, NotificationMessages.BudgetUpdationSuccess);
        }
    }

    async Task OnDeleteAsync(Models.Budget budget)
    {
        if (grid == null)
            return;

        var confirm = await DlgSvc.Confirm(
            "Are you sure?",
            $"Delete budget for {budget.CategoryDesc}",
            new ConfirmOptions() { OkButtonText = "Yes", CancelButtonText = "No" })
            .ConfigureAwait(false);
        if (confirm.HasValue && !confirm.Value)
            return;

        var result = await DeleteBudgetHandler.DoAsync(new(Year, budget.CategoryId)).ConfigureAwait(false);
        if (result.IsFailure)
        {
            Logger.LogError("Failed to delete budget: {Error}", result.Error);
            Notifier.Notify(NotificationSeverity.Error, NotificationMessages.BudgetDeletionFailed);
            return;
        }
        else
        {
            Data.Remove(budget);
            Models.Budget.RecalculateTotal(Data);
            // TODO needed to be fixed
            await RecalculateBudgetSummary.InvokeAsync().ConfigureAwait(false);
            Notifier.Notify(NotificationSeverity.Success, NotificationMessages.BudgetDeletionSuccess);
        }

        await grid.Reload();
    }

    enum Operation
    {
        Create, Update, Delete, None
    }
}
