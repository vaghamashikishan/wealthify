
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Telegram.Bot;
using wealthify.Database;
using wealthify.Extensions;
using wealthify.Middlewares;
using wealthify.Options;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Loggin - Serilog
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console();
});

var connectionString = builder.Configuration["ConnectionStrings:DefaultConnection"]!;

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException("Connection string 'DefaultConnection' is required.");
    }
    options.UseNpgsql(connectionString);
}
);

// Add health checks for the database
builder.Services.AddHealthChecks()
    .AddNpgSql(
        connectionString!,
        name: "PostgreSQL",
        timeout: TimeSpan.FromSeconds(5),
        tags: new[] { "db", "sql", "postgres" }
    );

// Adding Services, repositories here
builder.Services.AddApplicationServices();

builder.Services.AddScoped<GlobalExceptionHandlerMiddleware>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Setting up the options
builder.Services.Configure<TelegramBotOptions>(
    builder.Configuration.GetSection(TelegramBotOptions.SectionName));
builder.Services.Configure<ExpenseOptions>(
    builder.Configuration.GetSection(ExpenseOptions.SectionName));

// Telegram setup
builder.Services.AddSingleton<ITelegramBotClient>(sp =>
{
    var options = sp.GetRequiredService<IOptions<TelegramBotOptions>>().Value;
    if (string.IsNullOrWhiteSpace(options.Token))
    {
        throw new InvalidOperationException("TelegramBot:Token is required.");
    }

    return new TelegramBotClient(options.Token);
});

// Adding JWT Authentication
var envConfig = builder.Configuration.GetSection("JWT");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = envConfig["ISSUER"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(envConfig["SECRET_TOKEN"]!)),
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
        options.SwaggerEndpoint("/openapi/v1.json", "Wealthify API v1"));
}

// DB migration
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.UseHttpsRedirection();
app.UseSerilogRequestLogging();
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapHealthChecks("/health"); // Map health check endpoint
app.MapControllers();

app.Run();
