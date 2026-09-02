using Microsoft.EntityFrameworkCore;
using MyShop.DAL;
using Serilog;
using Serilog.Events; // Required for LogEventLevel
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ItemDbContext>(options => {
    options.UseSqlite(
        builder.Configuration["ConnectionStrings:ItemDbContextConnection"]);
});

builder.Services.AddDefaultIdentity<IdentityUser>(options => {
    options.SignIn.RequireConfirmedAccount = false; // (disable email confirmation requirement for easy local testing)
    }).AddEntityFrameworkStores<ItemDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/Login";
});

builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
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
app.UseSession(); // 4. Add Session Middleware (Must be placed before UseAuthentication)

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultControllerRoute();

app.Run();