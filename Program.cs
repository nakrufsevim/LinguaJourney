using System.Data;
using System.Data.Common;
using LinguaJourney.Data;
using LinguaJourney.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(1);
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");

    try
    {
        var dbContext = services.GetRequiredService<ApplicationDbContext>();
        BridgeEnsureCreatedDatabaseIfNeeded(dbContext);
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Database migration was skipped. Check the SQL Server connection settings if the app cannot read or write data.");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

static void BridgeEnsureCreatedDatabaseIfNeeded(ApplicationDbContext dbContext)
{
    var allMigrations = dbContext.Database.GetMigrations().ToList();
    if (allMigrations.Count != 1 || !dbContext.Database.CanConnect())
    {
        return;
    }

    using var connection = dbContext.Database.GetDbConnection();
    if (connection.State != ConnectionState.Open)
    {
        connection.Open();
    }

    if (TableExists(connection, "__EFMigrationsHistory") || !TableExists(connection, "LanguageTracks"))
    {
        return;
    }

    using var createHistoryTable = connection.CreateCommand();
    createHistoryTable.CommandText =
        """
        CREATE TABLE [__EFMigrationsHistory] (
            [MigrationId] nvarchar(150) NOT NULL,
            [ProductVersion] nvarchar(32) NOT NULL,
            CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
        );
        """;
    createHistoryTable.ExecuteNonQuery();

    using var insertInitialMigration = connection.CreateCommand();
    insertInitialMigration.CommandText =
        """
        INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
        VALUES (@migrationId, @productVersion);
        """;

    var migrationIdParameter = insertInitialMigration.CreateParameter();
    migrationIdParameter.ParameterName = "@migrationId";
    migrationIdParameter.Value = allMigrations[0];
    insertInitialMigration.Parameters.Add(migrationIdParameter);

    var productVersionParameter = insertInitialMigration.CreateParameter();
    productVersionParameter.ParameterName = "@productVersion";
    productVersionParameter.Value = "8.0.5";
    insertInitialMigration.Parameters.Add(productVersionParameter);

    insertInitialMigration.ExecuteNonQuery();
}

static bool TableExists(DbConnection connection, string tableName)
{
    using var command = connection.CreateCommand();
    command.CommandText =
        """
        SELECT COUNT(*)
        FROM INFORMATION_SCHEMA.TABLES
        WHERE TABLE_NAME = @tableName;
        """;

    var tableNameParameter = command.CreateParameter();
    tableNameParameter.ParameterName = "@tableName";
    tableNameParameter.Value = tableName;
    command.Parameters.Add(tableNameParameter);

    return Convert.ToInt32(command.ExecuteScalar()) > 0;
}
