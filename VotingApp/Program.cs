using Microsoft.EntityFrameworkCore;
using VotingApp.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Get Connection String
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// 2. CONFIGURE DB CONTEXT (Fixed Version for MySQL 8.0)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 32))));

builder.Services.AddControllersWithViews();

var app = builder.Build();

// 3. AUTOMATIC MIGRATIONS ON STARTUP (With Connection Retries)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        int retries = 5;
        while (retries > 0)
        {
            try 
            {
                context.Database.SetCommandTimeout(180);
                context.Database.Migrate(); 
                logger.LogInformation("✅ Database migrated successfully.");
                break; 
            }
            catch (Exception ex) 
            {
                retries--;
                if (retries == 0) throw;
                logger.LogWarning($"Waiting for MySQL... {retries} retries left. Error: {ex.Message}");
                Thread.Sleep(10000); // Wait 10s between attempts
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "❌ An error occurred while migrating the database.");
    }
}

// Standard Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
