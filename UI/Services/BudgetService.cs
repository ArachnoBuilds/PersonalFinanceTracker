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
                    { Header.Jan, 5000 },
                    { Header.Feb, 5000 },
                    { Header.Mar, 5000 },
                    { Header.Apr, 10_000 },
                    { Header.May, 10_000 },
                    { Header.Jun, 10_000 },
                    { Header.Jul, 10_000 },
                    { Header.Aug, 10_000 },
                    { Header.Sep, 10_000 },
                    { Header.Oct, 10_000 },
                    { Header.Nov, 10_000 },
                    { Header.Dec, 10_000 }
                }),
                new ("Freelance", new()
                {
                    { Header.Jan, 2000 },
                    { Header.Feb, 1500 },
                    { Header.Mar, 3000 },
                    { Header.Apr, 2500 },
                    { Header.May, 4000 },
                    { Header.Jun, 3500 },
                    { Header.Jul, 3000 },
                    { Header.Aug, 4500 },
                    { Header.Sep, 5000 },
                    { Header.Oct, 6000 },
                    { Header.Nov, 5500 },
                    { Header.Dec, 7000 }
                }),
                new ("LTA", new()
                {
                    { Header.Jan, 0 },
                    { Header.Feb, 0 },
                    { Header.Mar, 0 },
                    { Header.Apr, 0 },
                    { Header.May, 0 },
                    { Header.Jun, 0 },
                    { Header.Jul, 0 },
                    { Header.Aug, 0 },
                    { Header.Sep, 0 },
                    { Header.Oct, 0 },
                    { Header.Nov, 0 },
                    { Header.Dec, 15_000 }
                }),
                new("PLI", new()
                {
                    { Header.Jan, 0 },
                    { Header.Feb, 0 },
                    { Header.Mar, 15_500 },
                    { Header.Apr, 0 },
                    { Header.May, 0 },
                    { Header.Jun, 20_000 },
                    { Header.Jul, 0 },
                    { Header.Aug, 0 },
                    { Header.Sep, 20_000 },
                    { Header.Oct, 0 },
                    { Header.Nov, 0 },
                    { Header.Dec, 20_000 }
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
                    { Header.Jan, 5000 },
                    { Header.Feb, 5000 },
                    { Header.Mar, 5000 },
                    { Header.Apr, 5000 },
                    { Header.May, 5000 },
                    { Header.Jun, 5000 },
                    { Header.Jul, 5000 },
                    { Header.Aug, 5000 },
                    { Header.Sep, 5000 },
                    { Header.Oct, 5000 },
                    { Header.Nov, 5000 },
                    { Header.Dec, 5000 }
                }),
                new ("Utilities", new()
                {
                    { Header.Jan, 3000 },
                    { Header.Feb, 3000 },
                    { Header.Mar, 3000 },
                    { Header.Apr, 3000 },
                    { Header.May, 3000 },
                    { Header.Jun, 3000 },
                    { Header.Jul, 3000 },
                    { Header.Aug, 3000 },
                    { Header.Sep, 3000 },
                    { Header.Oct, 3000 },
                    { Header.Nov, 3000 },
                    { Header.Dec, 3000 }
                }),
                new ("Groceries", new()
                {
                    { Header.Jan, 4000 },
                    { Header.Feb, 4000 },
                    { Header.Mar, 4000 },
                    { Header.Apr, 4000 },
                    { Header.May, 4000 },
                    { Header.Jun, 4000 },
                    { Header.Jul, 4000 },
                    { Header.Aug, 4000 },
                    { Header.Sep, 4000 },
                    { Header.Oct, 4000 },
                    { Header.Nov, 4000 },
                    { Header.Dec, 4000 }
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
                    { Header.Jan, 1000 },
                    { Header.Feb, 1000 },
                    { Header.Mar, 1000 },
                    { Header.Apr, 1000 },
                    { Header.May, 1000 },
                    { Header.Jun, 1000 },
                    { Header.Jul, 1000 },
                    { Header.Aug, 1000 },
                    { Header.Sep, 1000 },
                    { Header.Oct, 1000 },
                    { Header.Nov, 1000 },
                    { Header.Dec, 1000 }
                }),
                new ("Retirement", new()
                {
                    { Header.Jan, 2000 },
                    { Header.Feb, 2000 },
                    { Header.Mar, 2000 },
                    { Header.Apr, 2000 },
                    { Header.May, 2000 },
                    { Header.Jun, 2000 },
                    { Header.Jul, 2000 },
                    { Header.Aug, 2000 },
                    { Header.Sep, 2000 },
                    { Header.Oct, 2000 },
                    { Header.Nov, 2000 },
                    { Header.Dec, 2000 }
                }),
                new ("Investments", new()
                {
                    { Header.Jan, 1500 },
                    { Header.Feb, 1500 },
                    { Header.Mar, 1500 },
                    { Header.Apr, 1500 },
                    { Header.May, 1500 },
                    { Header.Jun, 1500 },
                    { Header.Jul, 1500 },
                    { Header.Aug, 1500 },
                    { Header.Sep, 1500 },
                    { Header.Oct, 1500 },
                    { Header.Nov, 1500 },
                    { Header.Dec, 1500 }
                })
            ];
        return savings;
    }
}
