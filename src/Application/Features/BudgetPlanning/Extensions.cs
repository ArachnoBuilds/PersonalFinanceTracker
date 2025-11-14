using Application.Features.BudgetPlanning.Models;
using Application.Shared.Models;
using PM = Application.Shared.Persistence;

namespace Application.Features.BudgetPlanning;

internal static class Extensions
{
    extension(AnnualBudget annualBudget)
    {
        public PM.Category ToCategory(BudgetType type, int? id = null) => new()
        {
            Id = id ?? annualBudget.CategoryId,
            Description = annualBudget.CategoryDesc,
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
                    Amount = (double)annualBudget.Budgets.GetValueOrDefault(month, 0),
                    CategoryId = id ?? annualBudget.CategoryId
                });
            }
            return budgets;
        }
    }
}
