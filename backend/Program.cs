using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Services;
using Microsoft.Data.SqlClient;

// Load environment variables from .env file
var currentDirectory = System.IO.Directory.GetCurrentDirectory();
var dotenv = System.IO.Path.Combine(currentDirectory, ".env");
if (!System.IO.File.Exists(dotenv))
{
    var parentDirectory = System.IO.Directory.GetParent(currentDirectory)?.FullName;
    if (parentDirectory != null)
    {
        dotenv = System.IO.Path.Combine(parentDirectory, ".env");
    }
}

if (System.IO.File.Exists(dotenv))
{
    foreach (var line in System.IO.File.ReadAllLines(dotenv))
    {
        if (string.IsNullOrWhiteSpace(line) || line.Trim().StartsWith("#")) continue;
        var index = line.IndexOf('=');
        if (index > 0)
        {
            var key = line.Substring(0, index).Trim();
            var value = line.Substring(index + 1).Trim().Trim('"').Trim('\'');
            Environment.SetEnvironmentVariable(key, value);
        }
    }
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure ApplicationDbContext with SQL Server
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
                       ?? builder.Configuration.GetConnectionString("DefaultConnection");

connectionString = FindWorkingConnectionString(connectionString);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

string? FindWorkingConnectionString(string? primaryConnectionString)
{
    if (string.IsNullOrEmpty(primaryConnectionString)) return primaryConnectionString;

    try
    {
        var connBuilder = new SqlConnectionStringBuilder(primaryConnectionString);
        var originalServer = connBuilder.DataSource;

        var serverCandidates = new List<string>
        {
            originalServer,
            "localhost",
            "localhost\\SQLEXPRESS",
            "localhost\\MSSQLSERVER03",
            "localhost\\MSSQLSERVER",
            "localhost\\MSSQL",
            "(localdb)\\MSSQLLocalDB",
            "."
        };

        foreach (var server in serverCandidates.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (string.IsNullOrEmpty(server)) continue;

            var testBuilder = new SqlConnectionStringBuilder(primaryConnectionString)
            {
                DataSource = server,
                InitialCatalog = "master", // Connect to master to test server reachability
                ConnectTimeout = 2 // Low timeout for fast fallback
            };

            try
            {
                using (var conn = new SqlConnection(testBuilder.ConnectionString))
                {
                    conn.Open();
                    // Successfully connected to master, return the connection string with this server
                    var finalBuilder = new SqlConnectionStringBuilder(primaryConnectionString)
                    {
                        DataSource = server
                    };
                    return finalBuilder.ConnectionString;
                }
            }
            catch (SqlException)
            {
                // Try next candidate
            }
        }
    }
    catch
    {
        // Fallback to original if any parsing error occurs
    }

    return primaryConnectionString;
}

builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IManagerService, ManagerService>();
builder.Services.AddScoped<IAccountantService, AccountantService>();
builder.Services.AddScoped<IAdminService, AdminService>();

var frontendUrl = Environment.GetEnvironmentVariable("FRONTEND_URL") ?? "http://localhost:4200";
builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
    {
        policy.WithOrigins(frontendUrl)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var backendUrl = Environment.GetEnvironmentVariable("BACKEND_URL");
if (!string.IsNullOrEmpty(backendUrl))
{
    builder.WebHost.UseUrls(backendUrl);
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("frontend");
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        DbSeeder.Seed(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}

app.Run();

