using Application.Shared.Models;
using Components.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Radzen;
using Radzen.Blazor;

namespace Components.BudgetPlanning;

public partial class BudgetGrid
{
    [Parameter]
    public BudgetType Type { get; set; } = BudgetType.Summary;
    [Parameter]
    public List<Models.Budget> Data { get; set; } = [];
    [Parameter]
    public EventCallback<List<Models.Budget>> DataChanged { get; set; }
    [Parameter]
    public EventCallback RecalculateBudgetSummary { get; set; }

    RadzenDataGrid<Models.Budget>? grid;
    List<Category> categories = [];
    string FirstColumnHeader => Type switch
    {
        BudgetType.Summary => string.Empty,
        _ => Type.ToString()
    };
    GridOperation operation = GridOperation.None;
    string selectedCategoryDesc = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

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
    }

    void Reset()
    {
        operation = GridOperation.None;
        selectedCategoryDesc = string.Empty;
    }

    async Task OnAddRowAsync()
    {
        if (grid == null || !grid.IsValid || operation is not GridOperation.None)
        {
            Notifier.Notify(NotificationSeverity.Error, NotificationMessages.BudgetCreateInitializationFailed);
            return;
        }

        operation = GridOperation.Create;
        selectedCategoryDesc = string.Empty;
        await grid.InsertRow(new()
        {
            CategoryId = -1
        });
    }

    async Task OnEditRowAsync(Models.Budget budget)
    {
        if (grid == null || !grid.IsValid || operation is not GridOperation.None)
        {
            Notifier.Notify(NotificationSeverity.Error, NotificationMessages.BudgetUpdateInitializationFailed);
            return;
        }

        operation = GridOperation.Update;
        selectedCategoryDesc = budget.CategoryDesc;
        await grid.EditRow(budget);
    }

    async Task OnSaveRowAsync(Models.Budget budget)
    {
        if (grid == null || !grid.IsValid)
            return;

        // set category details based on operation
        switch (operation)
        {
            case GridOperation.Create:
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
            case GridOperation.Update:
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
        Reset();
    }

    async Task OnCancelEditAsync(Models.Budget budget)
    {
        if (grid == null)
            return;

        // cancel changes
        grid.CancelEditRow(budget);
        Reset();
    }

    async Task OnCreateAsync(Models.Budget budget)
    {
        if (operation is not GridOperation.Create)
            return;

        var annualBudget = budget.ToAnnualBudget();
        var result = await CreateBudgetHandler.DoAsync(new(State.Year, Type, annualBudget)).ConfigureAwait(false);
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
        Reset();
    }

    async Task OnUpdateAsync(Models.Budget budget)
    {
        if (operation is not GridOperation.Update)
            return;

        var annualBudget = budget.ToAnnualBudget();
        var result = await UpdateBudgetHandler.DoAsync(new(State.Year, annualBudget)).ConfigureAwait(false);
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
        Reset();
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

        var result = await DeleteBudgetHandler.DoAsync(new(State.Year, budget.CategoryId)).ConfigureAwait(false);
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
}
