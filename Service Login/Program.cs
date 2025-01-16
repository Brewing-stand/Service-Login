using Microsoft.EntityFrameworkCore;
using Refit;
using Service_Login.Context;
using Service_Login.Repositories;
using Service_Login.Services;
using Service_Login.Settings;

var builder = WebApplication.CreateBuilder(args);
var corsPolicy = "CorsPolicy";

// Load configuration from alternate appsettings file in production

if (builder.Environment.IsProduction())
{
    var environment = builder.Environment.EnvironmentName; // e.g., "Development", "Production"
    var configFileName = environment == "Production"
        ? "/mnt/secretprovider/appsettings-login-service" // Use secrets path in production
        : "appsettings.json";

    builder.Configuration
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile(configFileName, optional: false, reloadOnChange: true)
        .AddEnvironmentVariables();
}

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Settings
builder.Services.Configure<GitSecrets>(builder.Configuration.GetSection("GitSecrets"));

// Refit
builder.Services.AddRefitClient<IGitHubAuthApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://github.com"));

builder.Services.AddRefitClient<IGitHubServiceApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://api.github.com"));

// Database
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresSQL_DB")));

// Services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IGitLoginRepository, GitLoginRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();

// CORS
builder.Services.AddCors(options =>
    options.AddPolicy(corsPolicy, policy => policy
        .WithOrigins(builder.Configuration.GetSection("AppSettings:AllowedOrigins").Get<string[]>()!)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(corsPolicy);

app.UseAuthorization();

app.MapControllers();

app.Run();
