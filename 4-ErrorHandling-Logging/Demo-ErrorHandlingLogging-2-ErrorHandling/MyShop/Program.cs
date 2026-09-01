using Microsoft.EntityFrameworkCore;
using MyShop.DAL;
using Serilog;
using Serilog.Events; // Required for LogEventLevel

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ItemDbContext>(options => {
    options.UseSqlite(
        builder.Configuration["ConnectionStrings:ItemDbContextConnection"]);
});

builder.Services.AddScoped<IItemRepository, ItemRepository>();

builder.Services.AddSerilog((services, loggerConfiguration) => 
{
    loggerConfiguration
        .MinimumLevel.Information()
        .WriteTo.Console() // Commented out to reduce console noise during testing
        .WriteTo.File($"Logs/app_{DateTime.Now:yyyyMMdd_HHmmss}.log")
        // Filter out Information-level Entity Framework database execution logs
        .Filter.ByExcluding(e => e.Properties.TryGetValue("SourceContext", out var value) &&
                                e.Level == LogEventLevel.Information &&
                                e.MessageTemplate.Text.Contains("Executed DbCommand"));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    DBInit.Seed(app);
}

app.MapStaticAssets(); // Enable static assets from wwwroot (images, JS, CSS)

app.MapDefaultControllerRoute();

app.Run();