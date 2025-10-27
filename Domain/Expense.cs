namespace ExpenseApp.Domain;

public class Expense
{
    //id för varje utgift, ökar automatiskt
    public int Id { get; set; }
    //datum då utgiften gjordes
    public DateOnly Date { get; set; }
    //kategori t.ex. mat, resa, hushåll
    public string Category { get; set; } = "";
    //kort beskrivning valfri
    public string Description { get; set; } = "";
    //belopp i kr
    public decimal Amount { get; set; }

//
    public override string ToString()
    {
        return $"{Id,3} | {Date:yyyy-MM-dd} | {Category,-12} | {Amount,8:C} | {Description}";
    }
}
