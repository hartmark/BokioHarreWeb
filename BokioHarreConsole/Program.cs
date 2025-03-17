using System.Globalization;
using Common.Apis;
using Common.Handlers;
using Common.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using Spectre.Console;

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

var bokioApi = serviceProvider.GetRequiredService<IJournalService>();
var journalEntries = bokioApi.GetJournalEntries();

var table = new Table
{
    Title = new TableTitle("Journal Entries"),
    ShowHeaders = false,
    Border = TableBorder.None
};

table.AddColumn("");

await foreach (var item in journalEntries)
{
    var itemTable = new Table
    {
        ShowHeaders = false,
        Border = TableBorder.None
    };
    itemTable.AddColumn(string.Empty);
    itemTable.AddRow($"{item.JournalEntryNumber}: {item.Title}");
    itemTable.AddRow(item.Date.ToShortDateString());

    var subTable = new Table();

    if (item.Items == null)
    {
        continue;
    }
    
    subTable.AddColumn(new TableColumn("Account"));
    subTable.AddColumn(new TableColumn("Debit"));
    subTable.AddColumn(new TableColumn("Credit"));
    foreach (var subItem in item.Items!)
    {
        subTable.AddRow(
            subItem.Account.ToString(),
            GetFormattedAmount(subItem.Debit),
            GetFormattedAmount(subItem.Credit));
    }
    
    itemTable.AddRow(subTable);
    table.AddRow(itemTable);
}

AnsiConsole.Write(table);

return;

string GetFormattedAmount(decimal? value)
{
    return value is 0 or null ? string.Empty : $"{value:c}";
}
