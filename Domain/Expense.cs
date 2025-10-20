namespace ExpenseApp.Domain;

public class Expense
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public string Category { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal Amount { get; set; }

    public override string ToString()
    {
        return $"{Id,3} | {Date:yyyy-MM-dd} | {Category,-12} | {Amount,8:C} | {Description}";
    }
}
