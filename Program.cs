using ExpenseApp.Services;
using ExpenseApp.UI;
using System.Globalization;

namespace ExpenseApp
{
    class Program
    {
        static void Main()
        {
            //kronor i konsolen
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("sv-SE");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("sv-SE");

            var repo = new ExpenseRepository(Path.Combine("Data", "expenses.json"));
            var service = new ExpenseService(repo);
            var reports = new ReportService(service);

            var menu = new Menu(service, reports);
            menu.Run();
        }
    }
}
