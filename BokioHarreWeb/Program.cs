using System.Globalization;
using BokioHarreWeb.Hubs;
using Common.Apis;
using Common.Handlers;
using Common.Services;
using Microsoft.AspNetCore.Localization;
using Refit;

var builder = WebApplication.CreateBuilder(args);

var baseUri = builder.Configuration["BokioApiUrl"] ?? throw new InvalidOperationException();

builder.Services.AddTransient<TokenHandler>();
builder.Services.AddRefitClient<IBokioApi>()
    .ConfigureHttpClient(httpClient =>
    {
        httpClient.BaseAddress = new Uri(baseUri);
    })
    .AddHttpMessageHandler<TokenHandler>();

builder.Services.AddTransient<IJournalService, JournalService>();

builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        x =>
        {
            x.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

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

app.UseCors("AllowAllOrigins");

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapHub<ItemHub>("/itemHub");

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=JournalEntries}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();