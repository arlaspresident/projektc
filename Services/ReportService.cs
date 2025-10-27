using System.Text;
using System.Globalization;

namespace ExpenseApp.Services;

//skapar rapporter baserat på utifterna, har ingen egen data utan hämtar allt frpn expenseservice
public class ReportService
{
    //koppling till expenseservice
    private readonly ExpenseService _service;
    public ReportService(ExpenseService service) => _service = service;


//exporterar alla utgifter till en csv fil, summerar per månad/kategori
    public string ExportCsv(string path = "Data/report.csv")
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);


        var inv = CultureInfo.InvariantCulture;
        var sb = new StringBuilder();
        sb.AppendLine("Type,Key,Sum");

        foreach (var (ym, sum) in _service.SumByMonth())
            sb.AppendLine($"Month,{ym},{sum.ToString(inv)}");

        foreach (var (cat, sum) in _service.SumByCategory())
            sb.AppendLine($"Category,{cat},{sum.ToString(inv)}");

        File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
        return Path.GetFullPath(path);
    }
}
