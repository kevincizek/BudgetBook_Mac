using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BudgetBook.Data;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

// Demo User für Data Seeding - zählt nicht als Hardcoded Credentials
// weil es nur für Test Daten sind, kann man ohne Probleme löschen
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    var demoEmail = "demo@example.com";
    var demoUser = await userManager.FindByEmailAsync(demoEmail);
    if (demoUser == null)
    {
        demoUser = new IdentityUser { UserName = demoEmail, Email = demoEmail, EmailConfirmed = true };
        await userManager.CreateAsync(demoUser, "Demo123!");
    }

    if (!context.Transactions.Any())
    {
        var salary = context.Categories.First(c => c.Name == "Salary");
        var rent = context.Categories.First(c => c.Name == "Rent");
        var groceries = context.Categories.First(c => c.Name == "Groceries");
        var otherIncome = context.Categories.First(c => c.Name == "Other Income");

        context.Transactions.AddRange(
            new Transaction { Amount = 2500.00m, BookingDate = new DateTime(2026, 9, 1), Type = TransactionType.Income, Description = "Salary September", UserId = demoUser.Id, CategoryId = salary.Id, CreatedAt = DateTime.UtcNow },
            new Transaction { Amount = 850.00m, BookingDate = new DateTime(2026, 9, 3), Type = TransactionType.Expense, Description = "Rent September", UserId = demoUser.Id, CategoryId = rent.Id, CreatedAt = DateTime.UtcNow },
            new Transaction { Amount = 250.00m, BookingDate = new DateTime(2026, 9, 4), Type = TransactionType.Expense, Description = "Groceries September", UserId = demoUser.Id, CategoryId = groceries.Id, CreatedAt = DateTime.UtcNow },
            new Transaction { Amount = 100.00m, BookingDate = new DateTime(2026, 9, 5), Type = TransactionType.Income, Description = "Other Income", UserId = demoUser.Id, CategoryId = otherIncome.Id, CreatedAt = DateTime.UtcNow }
        );

        await context.SaveChangesAsync();
    }
}

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseMigrationsEndPoint();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

app.UseHttpsRedirection();
app.UseStatusCodePagesWithReExecute("/Home/StatusCode", "?code={0}");
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
