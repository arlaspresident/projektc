using ExpenseApp.Domain;

namespace ExpenseApp.Services;

public class ExpenseService
{

    //läser/sparar till json
    private readonly ExpenseRepository _repo;

    //aktuell lista av utgifter
    private List<Expense> _items;

    //enkelt id nummer som ökar för varje ny post
    private int _nextId;


//laddar befintliga poster från repo och sätter startvärde för nästa id
    public ExpenseService(ExpenseRepository repo)
    {
        _repo = repo;
        _items = _repo.Load();
        _nextId = _items.Any() ? _items.Max(x => x.Id) + 1 : 1;
    }

    //returnerar alla utgifter i read only
    public IReadOnlyList<Expense> All() => _items;
    
    //lägger till en ny utgift efter validering, sparar direkt
    public Expense Add(DateOnly date, string category, string description, decimal amount)
    {
        if (amount <= 0) throw new ArgumentException("Belopp måste vara > 0.");
        if (string.IsNullOrWhiteSpace(category)) throw new ArgumentException("Kategori krävs");
        var e = new Expense
        {
            Id = _nextId++,
            Date = date,
            Category = category.Trim(),
            Description = description?.Trim() ?? "",
            Amount = amount
        };
        _items.Add(e);
        _repo.Save(_items);
        return e;
    }

//tar bort en utgift efter id, returnerar true om något tas bort
    public bool Remove(int id)
    {
        var e = _items.FirstOrDefault(x => x.Id == id);
        if (e == null) return false;
        _items.Remove(e);
        _repo.Save(_items);
        return true;
    }
//filterar efter år och månad
    public IEnumerable<Expense> FilterByMonth(int year, int month)
        => _items.Where(x => x.Date.Year == year && x.Date.Month == month);

//filterar efter kategori
    public IEnumerable<Expense> FilterByCategory(string category)
        => _items.Where(x => x.Category.Equals(category, StringComparison.OrdinalIgnoreCase));

//summerar belopp per kategori
    public IEnumerable<(string Category, decimal Sum)> SumByCategory(IEnumerable<Expense>? source = null)
        => (source ?? _items)
           .GroupBy(x => x.Category)
           .Select(g => (g.Key, g.Sum(x => x.Amount)));

//summerar belopp per månad över alla utgifter
    public IEnumerable<(string YearMonth, decimal Sum)> SumByMonth()
        => _items
           .GroupBy(x => $"{x.Date:yyyy-MM}")
           .Select(g => (g.Key, g.Sum(x => x.Amount)))
           .OrderBy(x => x.Key);
}
