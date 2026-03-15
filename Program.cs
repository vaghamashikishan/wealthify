
using Microsoft.EntityFrameworkCore;
using wealthify.Database;
using wealthify.Extensions;
using wealthify.Middlewares;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration["ConnectionStrings:DefaultConnection"]!;

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString)
);

// Add health checks for the database
builder.Services.AddHealthChecks()
    .AddNpgSql(
        connectionString!,
        name: "PostgreSQL",
        timeout: TimeSpan.FromSeconds(5),
        tags: new[] { "db", "sql", "postgres" }
    );

builder.Services.AddApplicationServices();

builder.Services.AddScoped<GlobalExceptionHandlerMiddleware>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
        options.SwaggerEndpoint("/openapi/v1.json", "Wealthify API v1"));
}

app.UseHttpsRedirection();
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.UseAuthorization();
app.MapHealthChecks("/health"); // Map health check endpoint
app.MapControllers();

app.Run();
