using Application.Shared.Models;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;

namespace Components.BudgetPlanning;

public partial class BudgetGrid
{
    [Parameter]
    public BudgetType Type { get; set; } = BudgetType.Summary;
    [Parameter]
    public List<string> Categories { get; set; } = [];
    [Parameter]
    public List<Models.Budget> Data { get; set; } = [];
    [Parameter]
    public EventCallback<List<Models.Budget>> DataChanged { get; set; }
    [Parameter]
    public EventCallback RecalculateBudgetSummary { get; set; }


    RadzenDataGrid<Models.Budget>? grid;
    string FirstColumnHeader => Type switch
    {
        BudgetType.Income => "Income",
        BudgetType.Expenses => "Expenses",
        BudgetType.Savings => "Savings",
        _ => string.Empty
    };
    Models.Budget? editable;

    

    protected override void OnInitialized()
    {
        Categories.RemoveAll(p => Data.Exists(d => d.Category == p));
        base.OnInitialized();
    }

    async Task OnAddRowAsync()
    {
        if (grid == null || !grid.IsValid)
            return;

        await grid.InsertRow(new());
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

    async Task OnCancelEditAsync(Models.    Budget budget)
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
