using ViewModels;

namespace Services;

public class BudgetService
{
    public List<AnnualBudget> GetIncomes(int year)
    {
        // Implementation to retrieve income data for the specified year
        List<AnnualBudget> incomes = [
                new("Salary", new()
                {
                    { Month.Jan, 5000 },
                    { Month.Feb, 5000 },
                    { Month.Mar, 5000 },
                    { Month.Apr, 10_000 },
                    { Month.May, 10_000 },
                    { Month.Jun, 10_000 },
                    { Month.Jul, 10_000 },
                    { Month.Aug, 10_000 },
                    { Month.Sep, 10_000 },
                    { Month.Oct, 10_000 },
                    { Month.Nov, 10_000 },
                    { Month.Dec, 10_000 }
                }),
                new ("Freelance", new()
                {
                    { Month.Jan, 2000 },
                    { Month.Feb, 1500 },
                    { Month.Mar, 3000 },
                    { Month.Apr, 2500 },
                    { Month.May, 4000 },
                    { Month.Jun, 3500 },
                    { Month.Jul, 3000 },
                    { Month.Aug, 4500 },
                    { Month.Sep, 5000 },
                    { Month.Oct, 6000 },
                    { Month.Nov, 5500 },
                    { Month.Dec, 7000 }
                }),
                new ("LTA", new()
                {
                    { Month.Jan, 0 },
                    { Month.Feb, 0 },
                    { Month.Mar, 0 },
                    { Month.Apr, 0 },
                    { Month.May, 0 },
                    { Month.Jun, 0 },
                    { Month.Jul, 0 },
                    { Month.Aug, 0 },
                    { Month.Sep, 0 },
                    { Month.Oct, 0 },
                    { Month.Nov, 0 },
                    { Month.Dec, 15_000 }
                }),
                new("PLI", new()
                {
                    { Month.Jan, 0 },
                    { Month.Feb, 0 },
                    { Month.Mar, 15_500 },
                    { Month.Apr, 0 },
                    { Month.May, 0 },
                    { Month.Jun, 20_000 },
                    { Month.Jul, 0 },
                    { Month.Aug, 0 },
                    { Month.Sep, 20_000 },
                    { Month.Oct, 0 },
                    { Month.Nov, 0 },
                    { Month.Dec, 20_000 }
                })
            ];
        return incomes;
    }

    public List<AnnualBudget> GetExpenses(int year)
    {
        // Implementation to retrieve expense data for the specified year
        List<AnnualBudget> expenses = [
                new("Rent", new()
                {
                    { Month.Jan, 5000 },
                    { Month.Feb, 5000 },
                    { Month.Mar, 5000 },
                    { Month.Apr, 5000 },
                    { Month.May, 5000 },
                    { Month.Jun, 5000 },
                    { Month.Jul, 5000 },
                    { Month.Aug, 5000 },
                    { Month.Sep, 5000 },
                    { Month.Oct, 5000 },
                    { Month.Nov, 5000 },
                    { Month.Dec, 5000 }
                }),
                new ("Utilities", new()
                {
                    { Month.Jan, 3000 },
                    { Month.Feb, 3000 },
                    { Month.Mar, 3000 },
                    { Month.Apr, 3000 },
                    { Month.May, 3000 },
                    { Month.Jun, 3000 },
                    { Month.Jul, 3000 },
                    { Month.Aug, 3000 },
                    { Month.Sep, 3000 },
                    { Month.Oct, 3000 },
                    { Month.Nov, 3000 },
                    { Month.Dec, 3000 }
                }),
                new ("Groceries", new()
                {
                    { Month.Jan, 4000 },
                    { Month.Feb, 4000 },
                    { Month.Mar, 4000 },
                    { Month.Apr, 4000 },
                    { Month.May, 4000 },
                    { Month.Jun, 4000 },
                    { Month.Jul, 4000 },
                    { Month.Aug, 4000 },
                    { Month.Sep, 4000 },
                    { Month.Oct, 4000 },
                    { Month.Nov, 4000 },
                    { Month.Dec, 4000 }
                })
            ];
        return expenses;
    }

    public List<AnnualBudget> GetSavings(int year)
    {
        // Implementation to retrieve savings data for the specified year
        List<AnnualBudget> savings = [
                new("Emergency Fund", new()
                {
                    { Month.Jan, 1000 },
                    { Month.Feb, 1000 },
                    { Month.Mar, 1000 },
                    { Month.Apr, 1000 },
                    { Month.May, 1000 },
                    { Month.Jun, 1000 },
                    { Month.Jul, 1000 },
                    { Month.Aug, 1000 },
                    { Month.Sep, 1000 },
                    { Month.Oct, 1000 },
                    { Month.Nov, 1000 },
                    { Month.Dec, 1000 }
                }),
                new ("Retirement", new()
                {
                    { Month.Jan, 2000 },
                    { Month.Feb, 2000 },
                    { Month.Mar, 2000 },
                    { Month.Apr, 2000 },
                    { Month.May, 2000 },
                    { Month.Jun, 2000 },
                    { Month.Jul, 2000 },
                    { Month.Aug, 2000 },
                    { Month.Sep, 2000 },
                    { Month.Oct, 2000 },
                    { Month.Nov, 2000 },
                    { Month.Dec, 2000 }
                }),
                new ("Investments", new()
                {
                    { Month.Jan, 1500 },
                    { Month.Feb, 1500 },
                    { Month.Mar, 1500 },
                    { Month.Apr, 1500 },
                    { Month.May, 1500 },
                    { Month.Jun, 1500 },
                    { Month.Jul, 1500 },
                    { Month.Aug, 1500 },
                    { Month.Sep, 1500 },
                    { Month.Oct, 1500 },
                    { Month.Nov, 1500 },
                    { Month.Dec, 1500 }
                })
            ];
        return savings;
    }
}
