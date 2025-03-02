using System.Globalization;
using BokioHarreWeb.Apis;
using BokioHarreWeb.Handlers;
using Microsoft.AspNetCore.Localization;
using Refit;

var builder = WebApplication.CreateBuilder(args);

var baseUri = builder.Configuration["BokioApiUrl"] ?? throw new InvalidOperationException();

builder.Services.AddTransient<TokenHandler>();
builder.Services.AddRefitClient<IBokioApi>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(baseUri);
    })
    .AddHttpMessageHandler<TokenHandler>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

var supportedCultures = new[]
{
    new CultureInfo("sv-SE")
};

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("sv-SE"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=JournalEntries}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();