using Application.Features.BudgetPlanning.Models;

namespace Components.BudgetPlanning;

internal static class Extensions
{
    extension(List<AnnualBudget> budgets)
    {
        public List<Models.Budget> ToBudget()
        {
            Models.Budget total = new()
            {
                CategoryDesc = "Total"
            };
            List<Models.Budget> returnables = [];
            foreach (var p in budgets)
            {
                // convert AnnualBudget to Budget
                Models.Budget budget = new()
                {
                    CategoryDesc = p.CategoryDesc,
                    Jan = p.Budgets.GetValueOrDefault(Month.Jan, 0),
                    Feb = p.Budgets.GetValueOrDefault(Month.Feb, 0),
                    Mar = p.Budgets.GetValueOrDefault(Month.Mar, 0),
                    Apr = p.Budgets.GetValueOrDefault(Month.Apr, 0),
                    May = p.Budgets.GetValueOrDefault(Month.May, 0),
                    Jun = p.Budgets.GetValueOrDefault(Month.Jun, 0),
                    Jul = p.Budgets.GetValueOrDefault(Month.Jul, 0),
                    Aug = p.Budgets.GetValueOrDefault(Month.Aug, 0),
                    Sep = p.Budgets.GetValueOrDefault(Month.Sep, 0),
                    Oct = p.Budgets.GetValueOrDefault(Month.Oct, 0),
                    Nov = p.Budgets.GetValueOrDefault(Month.Nov, 0),
                    Dec = p.Budgets.GetValueOrDefault(Month.Dec, 0),
                };

                // calculate totals
                total.Jan += budget.Jan;
                total.Feb += budget.Feb;
                total.Mar += budget.Mar;
                total.Apr += budget.Apr;
                total.May += budget.May;
                total.Jun += budget.Jun;
                total.Jul += budget.Jul;
                total.Aug += budget.Aug;
                total.Sep += budget.Sep;
                total.Oct += budget.Oct;
                total.Nov += budget.Nov;
                total.Dec += budget.Dec;

                returnables.Add(budget);
            }

            returnables.Add(total);

            return returnables;
        }
    }
    extension(Models.Budget budget)
    {
        public static Models.Budget ToSummaryBudget(
            Models.Budget income,
            Models.Budget expense,
            Models.Budget saving)
        {
            if (!income.IsTotalCategory || !expense.IsTotalCategory || !saving.IsTotalCategory)
                throw new ArgumentException("All budgets must be total categories.");
            Models.Budget summary = new()
            {
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

        public static void RecalculateTotal(List<Models.Budget> budgets)
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

        public AnnualBudget ToAnnualBudget() =>
            new(budget.Category,
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
