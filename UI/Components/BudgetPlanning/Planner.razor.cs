using ViewModels;

namespace Components.BudgetPlanning;

public partial class Planner
{
    
    int[] years = [];
    int selectedYear;
    Dictionary<Month, MonthlyBudgetSummary> summaries = [];
    List<AnnualBudget> incomeGrid = [];
    List<AnnualBudget> expensesGrid = [];
    List<AnnualBudget> savingsGrid = [];

    protected override void OnInitialized()
    {
        years = [2025, 2026, 2027, 2028, 2029, 2030];
        selectedYear = 2025;
        summaries = new Dictionary<Month, MonthlyBudgetSummary>() 
        {
            { Month.Jan, new(50_000.00, 30_0000, 10_0000) },
            { Month.Feb, new(50_000.00, 30_0000, 10_0000) },
            { Month.Mar, new(50_000.00, 30_0000, 10_0000) },
            { Month.Apr, new(50_000.00, 30_0000, 10_0000) },
            { Month.May, new(50_000.00, 30_0000, 10_0000) },
            { Month.Jun, new(50_000.00, 30_0000, 10_0000) },
            { Month.Jul, new(50_000.00, 30_0000, 10_0000) },
            { Month.Aug, new(50_000.00, 30_0000, 10_0000) },
            { Month.Sep, new(50_000.00, 30_0000, 10_0000) },
            { Month.Oct, new(50_000.00, 30_0000, 10_0000) },
            { Month.Nov, new(50_000.00, 30_0000, 10_0000) },
            { Month.Dec, new(50_000.00, 30_0000, 10_0000) },
            { Month.Total, new(50_000.00 * 12, 30_0000 * 12, 10_0000 * 12) }
        };
        incomeGrid = [
                new("Salary", new Dictionary<Month, double>()
                {
                    { Month.Jan, 50_000.00 },
                    { Month.Feb, 50_000.00 },
                    { Month.Mar, 50_000.00 },
                    { Month.Apr, 50_000.00 },
                    { Month.May, 50_000.00 },
                    { Month.Jun, 50_000.00 },
                    { Month.Jul, 50_000.00 },
                    { Month.Aug, 50_000.00 },
                    { Month.Sep, 50_000.00 },
                    { Month.Oct, 50_000.00 },
                    { Month.Nov, 50_000.00 },
                    { Month.Dec, 50_000.00 },
                    { Month.Total, 50_000.00*12 }
                }),
                new("PLI", new Dictionary<Month, double>()
                {
                    { Month.Jan, 0.00 },
                    { Month.Feb, 0.00 },
                    { Month.Mar, 45_000.00 },
                    { Month.Apr, 0.00 },
                    { Month.May, 0.00 },
                    { Month.Jun, 45_000.00 },
                    { Month.Jul, 0.00 },
                    { Month.Aug, 0.00 },
                    { Month.Sep, 45_000.00 },
                    { Month.Oct, 0.00 },
                    { Month.Nov, 0.00 },
                    { Month.Dec, 45_000.00 },
                    { Month.Total, 45_000.00*4 }
                })
            ];
        expensesGrid = [
                new("Household", new Dictionary<Month, double>()
                {
                    { Month.Jan, 20_000.00 },
                    { Month.Feb, 20_000.00 },
                    { Month.Mar, 20_000.00 },
                    { Month.Apr, 20_000.00 },
                    { Month.May, 20_000.00 },
                    { Month.Jun, 20_000.00 },
                    { Month.Jul, 20_000.00 },
                    { Month.Aug, 20_000.00 },
                    { Month.Sep, 20_000.00 },
                    { Month.Oct, 20_000.00 },
                    { Month.Nov, 20_000.00 },
                    { Month.Dec, 20_000.00 },
                    { Month.Total, 20_000.00*12 }
                }),
                new("Health Insurance", new Dictionary<Month, double>()
                {
                    { Month.Jan, 0.00 },
                    { Month.Feb, 0.00 },
                    { Month.Mar, 5_000.00 },
                    { Month.Apr, 0.00 },
                    { Month.May, 0.00 },
                    { Month.Jun, 5_000.00 },
                    { Month.Jul, 0.00 },
                    { Month.Aug, 0.00 },
                    { Month.Sep, 5_000.00 },
                    { Month.Oct, 0.00 },
                    { Month.Nov, 0.00 },
                    { Month.Dec, 5_000.00 },
                    { Month.Total, 5_000.00*4 }
                })
            ];
        savingsGrid = [
                new("SIP", new Dictionary<Month, double>()
                {
                    { Month.Jan, 15_000.00 },
                    { Month.Feb, 15_000.00 },
                    { Month.Mar, 15_000.00 },
                    { Month.Apr, 15_000.00 },
                    { Month.May, 15_000.00 },
                    { Month.Jun, 15_000.00 },
                    { Month.Jul, 15_000.00 },
                    { Month.Aug, 15_000.00 },
                    { Month.Sep, 15_000.00 },
                    { Month.Oct, 15_000.00 },
                    { Month.Nov, 15_000.00 },
                    { Month.Dec, 15_000.00 },
                    { Month.Total, 15_000.00*12 }
                }),
                new("Bonus", new Dictionary<Month, double>()
                {
                    { Month.Jan, 0.00 },
                    { Month.Feb, 0.00 },
                    { Month.Mar, 3_000.00 },
                    { Month.Apr, 0.00 },
                    { Month.May, 0.00 },
                    { Month.Jun, 3_000.00 },
                    { Month.Jul, 0.00 },
                    { Month.Aug, 0.00 },
                    { Month.Sep, 3_000.00 },
                    { Month.Oct, 0.00 },
                    { Month.Nov, 0.00 },
                    { Month.Dec, 3_000.00 },
                    { Month.Total, 5_000.00*4 }
                })
            ];
        base.OnInitialized();
    }
}