using Refit;
using Service_Login.Repositories;
using Service_Login.Services;
using Service_Login.Settings;

var builder = WebApplication.CreateBuilder(args);
var corsPolicy = "CorsPolicy";


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

builder.Services.AddRefitClient<IUserServiceApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(builder.Configuration["UserService:Url"]));

// Services
builder.Services.AddScoped<IUserServiceRepository, UserServiceRepository>();
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
