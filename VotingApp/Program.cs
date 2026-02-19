using Microsoft.EntityFrameworkCore;
using VotingApp.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Get Connection String
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// 2. CONFIGURE DB CONTEXT (Fix: Use fixed version instead of AutoDetect)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 32))));

builder.Services.AddControllersWithViews();

var app = builder.Build();

// 3. AUTOMATIC MIGRATIONS ON STARTUP (Fix: Retries for Connection)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        
        // Wait and retry up to 5 times if MySQL is still booting
        int retries = 5;
        while (retries > 0)
        {
            try {
                context.Database.Migrate(); // This creates the tables!
                logger.LogInformation("✅ Database migrated successfully.");
                break; 
            }
            catch (Exception) {
                retries--;
                if (retries == 0) throw;
                logger.LogWarning("Waiting for MySQL... retrying.");
                Thread.Sleep(5000);
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
