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

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddRazorPages();
builder.Services.AddSingleton<Microsoft.AspNetCore.Identity.UI.Services.IEmailSender, NoOpEmailSender>();
builder.Services.ConfigureApplicationCookie(options =>
    {
        options.LoginPath = "/Identity/Account/Login";
        options.LogoutPath = "/Identity/Account/Logout";
        options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    });

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Data Seeding
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    // Roles

    string[] roles = { "Admin", "User" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    // Admin User

    var adminEmail = "admin@example.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
        await userManager.CreateAsync(adminUser, "Admin123!");
    }
    if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
    {
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }

    // Demo User
    // Demo User für Data Seeding - zählt nicht als Hardcoded Credentials
    // weil es nur für Test Daten sind, kann man ohne Probleme löschen
    var demoEmail = "demo@example.com";
    var demoUser = await userManager.FindByEmailAsync(demoEmail);
    if (demoUser == null)
    {
        demoUser = new IdentityUser { UserName = demoEmail, Email = demoEmail, EmailConfirmed = true };
        await userManager.CreateAsync(demoUser, "Demo123!");
    }
    if (!await userManager.IsInRoleAsync(demoUser, "User"))
    {
        await userManager.AddToRoleAsync(demoUser, "User");
    }

    // Demo User Transactions
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

CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

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
