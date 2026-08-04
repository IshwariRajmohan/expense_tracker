using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Services;

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
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

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

app.Run();
