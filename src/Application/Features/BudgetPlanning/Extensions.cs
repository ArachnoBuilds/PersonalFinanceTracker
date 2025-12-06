using Application.Schema.BudgetPlanning.Models;
using Application.Schema.Shared.Models;
using PM = Application.Shared.Persistence;

namespace Application.Features.BudgetPlanning;

internal static class Extensions
{
    extension(Budget budget)
    {
        public PM.Category ToCategory(BudgetItemType type, int? id = null) => new()
        {
            Id = id ?? budget.BudgetItemId,
            Description = budget.BudgetItemDesc,
            Type = type.ToString()
        };

        public List<PM.Budget> ToBudgets(int year, int? id = null)
        {
            List<PM.Budget> budgets = [];
            var months = Enum.GetValues<Month>();
            for (int i = 0; i < months.Length; i++)
            {
                var month = months[i];
                budgets.Add(new PM.Budget
                {
                    Id = Guid.NewGuid().ToString(),
                    Year = year,
                    Month = (int)month,
                    Amount = (double)budget.MonthlyAmounts.GetValueOrDefault(month, 0),
                    CategoryId = id ?? budget.BudgetItemId
                });
            }
            return budgets;
        }
    }
}
