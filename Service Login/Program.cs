using Service_Template.Repositories;
using Service_Template.Settings;

var builder = WebApplication.CreateBuilder(args);
var corsPolicy = "CorsPolicy";


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();

// SECRETS
builder.Services.Configure<GitSecrets>(builder.Configuration.GetSection("GitSecrets"));

// Services
builder.Services.AddScoped<IGitLogin, GitLogin>();

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
