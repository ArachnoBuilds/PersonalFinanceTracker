using ViewModels;

namespace Components.BudgetPlanning;

internal static class Extensions
{
    extension(List<AnnualBudget> budgets)
    {
        public void CalculateTotals()
        {
            AnnualBudget total = new("Total", []);
            foreach (var p in budgets)
            {
                // calculate total for category
                p.Budget[Header.Total] = p.Budget.Values.Sum();

                // calculate running total for each header
                foreach (var k in p.Budget.Keys)
                {
                    var amt = total.Budget.TryGetValue(k, out var val) ? val : 0;
                    total.Budget[k] = amt + p.Budget[k];
                }
            }
            budgets.Add(total);
        }
    }

    extension(MonthlyBudgetSummary)
    {
        public static Dictionary<Header, MonthlyBudgetSummary> Calculate(
            List<AnnualBudget> incomes,
            List<AnnualBudget> expenses,
            List<AnnualBudget> savings)
        {
            Dictionary<Header, MonthlyBudgetSummary> summaries = [];
            var headers = Enum.GetValues<Header>();
            for (int i = 0; i < headers.Length; i++)
            {
                var header = headers[i];

                // calulate total income
                double totalIncome = 0;
                var income = incomes.Find(p => p.Category == "Total");
                if (income == null)
                {
                    incomes.CalculateTotals();
                    income = incomes.Find(p => p.Category == "Total");
                }
                if (income != null && income.Budget.TryGetValue(header, out var incomeValue))
                    totalIncome = incomeValue;

                // calculate total expenses
                double totalExpenses = 0;
                var expense = expenses.Find(p => p.Category == "Total");
                if (expense == null)
                {
                    expenses.CalculateTotals();
                    expense = expenses.Find(p => p.Category == "Total");
                }
                if (expense != null && expense.Budget.TryGetValue(header, out var expenseValue))
                    totalExpenses = expenseValue;

                // calculate total savings
                double totalSavings = 0;
                var saving = savings.Find(p => p.Category == "Total");
                if (saving == null)
                {
                    savings.CalculateTotals();
                    saving = savings.Find(p => p.Category == "Total");
                }
                if (saving != null && saving.Budget.TryGetValue(header, out var savingValue))
                    totalSavings = savingValue;

                summaries[header] = new(totalIncome, totalExpenses, totalSavings);
            }

            return summaries;
        }
    }
}
