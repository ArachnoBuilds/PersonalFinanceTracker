using Application.Schema.BudgetPlanning.Models;

namespace Components.BudgetPlanning;

internal static class Extensions
{
    extension(BudgetInfo budget)
    {
        public static BudgetInfo ToSummaryBudget(
            BudgetInfo income,
            BudgetInfo expense,
            BudgetInfo saving)
        {
            if (!income.IsTotalCategory || !expense.IsTotalCategory || !saving.IsTotalCategory)
                throw new ArgumentException("All budgets must be total categories.");
            BudgetInfo summary = new()
            {
                BudgetItemId = -1,
                Jan = income.Jan - expense.Jan - saving.Jan,
                Feb = income.Feb - expense.Feb - saving.Feb,
                Mar = income.Mar - expense.Mar - saving.Mar,
                Apr = income.Apr - expense.Apr - saving.Apr,
                May = income.May - expense.May - saving.May,
                Jun = income.Jun - expense.Jun - saving.Jun,
                Jul = income.Jul - expense.Jul - saving.Jul,
                Aug = income.Aug - expense.Aug - saving.Aug,
                Sep = income.Sep - expense.Sep - saving.Sep,
                Oct = income.Oct - expense.Oct - saving.Oct,
                Nov = income.Nov - expense.Nov - saving.Nov,
                Dec = income.Dec - expense.Dec - saving.Dec
            };
            return summary;
        }

        public static void RecalculateTotal(List<BudgetInfo> budgets)
        {
            var totalBudget = budgets.FirstOrDefault(b => b.IsTotalCategory);
            if (totalBudget == null)
                return;

            var otherBudgets = budgets.Where(b => !b.IsTotalCategory);
            totalBudget.Jan = otherBudgets.Sum(b => b.Jan);
            totalBudget.Feb = otherBudgets.Sum(b => b.Feb);
            totalBudget.Mar = otherBudgets.Sum(b => b.Mar);
            totalBudget.Apr = otherBudgets.Sum(b => b.Apr);
            totalBudget.May = otherBudgets.Sum(b => b.May);
            totalBudget.Jun = otherBudgets.Sum(b => b.Jun);
            totalBudget.Jul = otherBudgets.Sum(b => b.Jul);
            totalBudget.Aug = otherBudgets.Sum(b => b.Aug);
            totalBudget.Sep = otherBudgets.Sum(b => b.Sep);
            totalBudget.Oct = otherBudgets.Sum(b => b.Oct);
            totalBudget.Nov = otherBudgets.Sum(b => b.Nov);
            totalBudget.Dec = otherBudgets.Sum(b => b.Dec);
        }

        public Budget ToBudget() =>
            new(budget.BudgetItemId,
                budget.BudgetItemDesc,
                new()
                {
                    [Month.Jan] = budget.Jan,
                    [Month.Feb] = budget.Feb,
                    [Month.Mar] = budget.Mar,
                    [Month.Apr] = budget.Apr,
                    [Month.May] = budget.May,
                    [Month.Jun] = budget.Jun,
                    [Month.Jul] = budget.Jul,
                    [Month.Aug] = budget.Aug,
                    [Month.Sep] = budget.Sep,
                    [Month.Oct] = budget.Oct,
                    [Month.Nov] = budget.Nov,
                    [Month.Dec] = budget.Dec
                });
    }
}