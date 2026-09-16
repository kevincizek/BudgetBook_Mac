public class StatisticsViewModel
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal Saldo => TotalIncome - TotalExpense;

    public List<CategorySummary> ExpensesByCategory { get; set; } = new();
    public List<MonthlySummary> TransactionsByMonth { get; set; } = new();
}

public class CategorySummary
{
    public string CategoryName { get; set; } = "";
    public decimal Total { get; set; }
}

public class MonthlySummary
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal Income { get; set; }
    public decimal Expense { get; set; }

    public string Label => new DateTime(Year, Month, 1).ToString("MMMM yyyy");
}
