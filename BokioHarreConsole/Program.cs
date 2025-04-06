using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Common.Apis;
using Common.Extensions;
using Common.Handlers;
using Common.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using Spectre.Console;
using StringExtensions = Common.Extensions.StringExtensions;

var cultureInfo = new CultureInfo("sv-SE");
Thread.CurrentThread.CurrentCulture = cultureInfo;
Thread.CurrentThread.CurrentUICulture = cultureInfo;

var serviceCollection = new ServiceCollection();

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddUserSecrets<Program>()
    .Build();
serviceCollection.AddSingleton<IConfiguration>(configuration);

serviceCollection.AddTransient<TokenHandler>();
serviceCollection.AddRefitClient<IBokioApi>()
    .ConfigureHttpClient((serviceProvider, httpClient) =>
    {
        var configuration2 = serviceProvider.GetRequiredService<IConfiguration>();
        var bokioApiUrl = configuration2["BokioApiUrl"];
        if (bokioApiUrl == null)
        {
            throw new InvalidOperationException();
        }

        httpClient.BaseAddress = new Uri(bokioApiUrl);
    })
    .AddHttpMessageHandler<TokenHandler>();

serviceCollection.AddTransient<IJournalService, JournalService>();

var serviceProvider = serviceCollection.BuildServiceProvider();

int[] borrowAccounts = [2829, 2893, 2892, 2898];

var bokioApi = serviceProvider.GetRequiredService<IJournalService>();
var journalEntries = bokioApi.GetJournalEntries()
    .ToBlockingEnumerable()
    .Where(x => x.Date.Year >= 2024)
    .Where(x => x.ReversingJournalEntryId is null)
    .Where(x => x.Items != null && x.Items.Any(y => borrowAccounts.Any(z => z == y.Account)))
    .OrderBy(x => x.Date)
    .Select(x =>
    {
        var items = x.Items!
            .Single(y => borrowAccounts.Any(z => z == y.Account));
        return new
        {
            x.Date,
            x.JournalEntryNumber,
            Title = (x.Title ?? string.Empty).Unescape(),
            items.Account,
            items.Debit,
            items.Credit,
            Rest = x
        };
    })
    .ToList();

var table = new Table
{
    Title = new TableTitle("Journal Entries")
};

table.AddColumn(new TableColumn("Datum"));
table.AddColumn(new TableColumn("Ver"));
table.AddColumn(new TableColumn("Beskrivning"));
table.AddColumn(new TableColumn("Konto"));
table.AddColumn(new TableColumn("Debet"));
table.AddColumn(new TableColumn("Kredit"));

foreach (var item in journalEntries)
{
    table.AddRow(
        item.Date.ToShortDateString(),
        item.JournalEntryNumber!,
        item.Title,
        item.Account.ToString(),
        GetFormattedAmount(item.Debit),
        GetFormattedAmount(item.Credit)
    );

}
table.Caption = new TableTitle($"Antal rader: {journalEntries.Count}"); 
AnsiConsole.Write(table);

return;

string GetFormattedAmount(decimal? value)
{
    return value is 0 or null ? string.Empty : $"{value:c}";
}
