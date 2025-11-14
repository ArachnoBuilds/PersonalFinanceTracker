using Application.Features.BudgetPlanning.Models;
using Application.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
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
        categories.RemoveAll(p => Data.Exists(d => d.CategoryDesc == p.Description));
        await base.OnInitializedAsync();
    }

    async Task OnAddRowAsync()
    {
        if (grid == null || !grid.IsValid)
            return;

        await grid.InsertRow(new()
        {
            CategoryId = -1
        });
    }

    async Task OnEditRowAsync(Models.Budget budget)
    {
        if (grid == null || !grid.IsValid)
            return;

        editable = budget;
        await grid.EditRow(budget);
    }

    async Task OnSaveRowAsync(Models.Budget budget)
    {
        if (grid == null || !grid.IsValid)
            return;

        await grid.UpdateRow(budget);
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
        var annualBudget = budget.ToAnnualBudget();
        var result = await CreateBudgetHandler.DoAsync(new(Year, Type, annualBudget)).ConfigureAwait(false);
        if (result.IsFailure)
        {
            Logger.LogError("Failed to create budget: {Error}", result.Error);
            Notifier.Notify(Radzen.NotificationSeverity.Error, NotificationMessages.BudgetCreationFailed);
            return;
        }
        Data.Add(budget);
        Models.Budget.RecalculateTotal(Data);
        await RecalculateBudgetSummary.InvokeAsync();
    }

    async Task OnUpdateAsync(Models.Budget budget)
    {
        if (editable == null)
            return;

        var idx = Data.IndexOf(editable);
        Data.Remove(editable);
        Data.Insert(idx, budget);
        Models.Budget.RecalculateTotal(Data);
        editable = null;
        await RecalculateBudgetSummary.InvokeAsync();
    }

    async Task OnDeleteAsync(Models.Budget budget)
    {
        if (grid == null)
            return;

        Data.Remove(budget);
        Models.Budget.RecalculateTotal(Data);
        await RecalculateBudgetSummary.InvokeAsync();

        await grid.Reload();
    }
}
