namespace Components.BudgetPlanning.Models;

public record Budget
{
    public int CategoryId { get; set; }
    public string CategoryDesc { get; set; } = string.Empty;
    public decimal Jan { get; set; } = 0.00m;
    public decimal Feb { get; set; } = 0.00m;
    public decimal Mar { get; set; } = 0.00m;
    public decimal Apr { get; set; } = 0.00m;
    public decimal May { get; set; } = 0.00m;
    public decimal Jun { get; set; } = 0.00m;
    public decimal Jul { get; set; } = 0.00m;
    public decimal Aug { get; set; } = 0.00m;
    public decimal Sep { get; set; } = 0.00m;
    public decimal Oct { get; set; } = 0.00m;
    public decimal Nov { get; set; } = 0.00m;
    public decimal Dec { get; set; } = 0.00m;
    public decimal Total => Jan + Feb + Mar + Apr + May + Jun + Jul + Aug + Sep + Oct + Nov + Dec;
    public bool IsTotalCategory => CategoryDesc.Equals("Total", StringComparison.InvariantCultureIgnoreCase);

    public static Budget EmptyTotal => new()
    {
        CategoryId = -1,
        CategoryDesc = "Total"
    };
}