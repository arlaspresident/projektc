using System.Text.Json;
using ExpenseApp.Domain;

namespace ExpenseApp.Services;

//hanterar lagring av utgifter till och från en json fil
public class ExpenseRepository
{
    //sökväg till json där utgifterna sparas
    private readonly string _path;
    //alternativ för json, writeindented gör filen lättare o läsa
    private readonly JsonSerializerOptions _opts = new() { WriteIndented = true };

//tar emot sökvägen till json filen som används
    public ExpenseRepository(string path) => _path = path;

//läser alla utgifter från json filen. om filen saknas eller är tom returneras en tom lista ist för fel
    public List<Expense> Load()
    {
        if (!File.Exists(_path)) return new List<Expense>();
        var json = File.ReadAllText(_path);
        return string.IsNullOrWhiteSpace(json)
            ? new List<Expense>()
            : (JsonSerializer.Deserialize<List<Expense>>(json, _opts) ?? new List<Expense>());
    }

//sparar en lista av utgifter till json. om mappen saknas skapas den automatiskt
    public void Save(List<Expense> items)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        var json = JsonSerializer.Serialize(items, _opts);
        File.WriteAllText(_path, json);
    }
}
