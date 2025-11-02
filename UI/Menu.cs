using ExpenseApp.Services;
using ExpenseApp.Domain;

namespace ExpenseApp.UI;

public class Menu
{
    private readonly ExpenseService _service;
    private readonly ReportService _reports;

    public Menu(ExpenseService service, ReportService reports)
    {
        _service = service;
        _reports = reports;
    }

    public void Run()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Expense Tracker ===");
            Console.WriteLine("1. Lista utgifter");
            Console.WriteLine("2. Lägg till utgift");
            Console.WriteLine("3. Ta bort utgift");
            Console.WriteLine("4. Filtrera per månad");
            Console.WriteLine("5. Filtrera per kategori");
            Console.WriteLine("6. Rapport (summa per månad & kategori)");
            Console.WriteLine("7. Exportera rapport till CSV");
            Console.WriteLine("8. Avsluta");
            Console.Write("Val: ");
            var choice = Console.ReadLine();

            try
            {
                switch (choice)
                {
                    case "1": ListAll(); break;
                    case "2": AddExpense(); break;
                    case "3": RemoveExpense(); break;
                    case "4": FilterByMonth(); break;
                    case "5": FilterByCategory(); break;
                    case "6": Report(); break;
                    case "7": ExportCsv(); break;
                    case "8": return;
                    default: Pause("Ogiltigt val."); break;
                }
            }
            catch (Exception ex)
            {
                //enkel felhantering så programmet inte kraschar vid fel input
                Pause($"Fel: {ex.Message}");
            }
        }
    }

//visar alla utgifter i tabellform
    private void ListAll()
    {
        Console.Clear();
        var items = _service.All().OrderByDescending(x => x.Date).ThenByDescending(x => x.Id).ToList();
        if (!items.Any()) { Pause("Inga utgifter ännu."); return; }
        PrintTable(items);
        Pause();
    }

//lägger till en ny utgift
    private void AddExpense()
    {
        Console.Clear();
        Console.WriteLine("=== Lägg till utgift ===");
        var date = ReadDate("Datum (YYYY-MM-DD): ");
        var category = ReadNonEmpty("Kategori: ");
        Console.Write("Beskrivning (valfritt): ");
        var desc = Console.ReadLine() ?? "";
        var amount = ReadDecimal("Belopp (t.ex. 199.90): ");
        var e = _service.Add(date, category, desc, amount);
        Pause($"Lagt till: {e}");
    }

    private void RemoveExpense()
    {
        Console.Clear();
        var id = ReadInt("Ange ID att ta bort: ");
        var ok = _service.Remove(id);
        Pause(ok ? "Borttagen." : "Hittade ej ID.");
    }

    private void FilterByMonth()
    {
        Console.Clear();
        var year = ReadInt("År (YYYY): ");
        var month = ReadInt("Månad (1-12): ");
        var items = _service.FilterByMonth(year, month).OrderByDescending(x => x.Date).ToList();
        if (!items.Any()) { Pause("Inga utgifter för vald månad"); return; }
        PrintTable(items);
        Console.WriteLine($"\nSumma för {year}-{month:00}: {items.Sum(x => x.Amount):C}");
        Pause();
    }

    private void FilterByCategory()
    {
        Console.Clear();
        var category = ReadNonEmpty("Kategori: ");
        var items = _service.FilterByCategory(category).OrderByDescending(x => x.Date).ToList();
        if (!items.Any()) { Pause("Inga utgifter i denna kategori"); return; }
        PrintTable(items);
        Console.WriteLine($"\nSumma i {category}: {items.Sum(x => x.Amount):C}");
        Pause();
    }

    private void Report()
    {
        Console.Clear();
        Console.WriteLine("=== Summa per månad ===");
        foreach (var (ym, sum) in _service.SumByMonth())
            Console.WriteLine($"{ym}: {sum:C}");

        Console.WriteLine("\n=== Summa per kategori (alla) ===");
        foreach (var (cat, sum) in _service.SumByCategory())
            Console.WriteLine($"{cat,-12} {sum,10:C}");

        Pause();
    }

    // helpers
    private static void PrintTable(IEnumerable<Expense> items)
    {
        Console.WriteLine(" ID | Datum      | Kategori     |    Belopp | Beskrivning");
        Console.WriteLine("----+------------+--------------+-----------+---------------------");
        foreach (var e in items) Console.WriteLine(e);
    }

    private static DateOnly ReadDate(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var s = Console.ReadLine();
            if (DateOnly.TryParse(s, out var d)) return d;
            Console.WriteLine("Ogiltigt datum försök igen");
        }
    }

    private static decimal ReadDecimal(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var s = Console.ReadLine();
            if (decimal.TryParse(s, out var v) && v > 0) return v;
            Console.WriteLine("Ogiltigt belopp (> 0) försök igen");
        }
    }

    private static int ReadInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var s = Console.ReadLine();
            if (int.TryParse(s, out var v)) return v;
            Console.WriteLine("Ogiltigt heltal försök igen");
        }
    }

    private static string ReadNonEmpty(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var s = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(s)) return s.Trim();
            Console.WriteLine("Fältet får inte va tomt");
        }
    }

    private static void Pause(string msg = "Tryck valfri tangent för att fortsätta")
    {
        Console.WriteLine(msg);
        Console.ReadKey(true);
    }
    private void ExportCsv()
    {
        Console.Clear();
        var fullPath = _reports.ExportCsv();
        Pause($"Rapport exporterad till:\n{fullPath}");
    }

}
