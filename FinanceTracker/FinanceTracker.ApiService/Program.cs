using FinanceTracker.ApiService.Data;
using FinanceTracker.ApiService.Services;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using FinanceTracker.ApiService.Controllers;
using FinanceTracker.ApiService.ExceptionsHandler;
using System.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using FinanceTracker.ApiService.Endpoints;
using FinanceTracker.ApiService.Services.Transaction;
using FinanceTracker.ApiService.Services.Category;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddProblemDetails(o =>
{
    o.CustomizeProblemDetails = (context) =>
    {
        context.ProblemDetails.Instance = $"{context.HttpContext.Request?.Method} {context.HttpContext.Request?.Path.Value}";
        context.ProblemDetails.Extensions.Add("requestId", context.HttpContext.TraceIdentifier);
        // Use the current Activity for trace id when available
        var activity = Activity.Current;
        context.ProblemDetails.Extensions.Add("traceId1", activity?.Id);
    };
});
builder.Services.AddExceptionHandler<Global_ExceptionHandler>();
// Register OpenAPI generator (basic). Custom document transformers referencing
// Microsoft.OpenApi.Models were removed to avoid hard dependency on that package.
builder.Services.AddOpenApi();

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();
builder.Services.AddAuthorization();

// Register EF Core DbContext using Npgsql (configured for .NET 10 / EF Core 10)
{
    var conn = builder.Configuration.GetConnectionString("postgresdb") ?? builder.Configuration["postgresdb"];
    if (string.IsNullOrWhiteSpace(conn))
    {
        throw new InvalidOperationException("Postgres connection string not configured. Please set ConnectionStrings:postgresdb or postgresdb in configuration.");
    }

    builder.Services.AddDbContext<FinanceDBContext>(options =>
    {
        options.UseNpgsql(conn);
    });
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["AppSettings:Issuer"] ?? "FinanceTrackerApi",
            ValidAudience = builder.Configuration["AppSettings:Audience"] ?? "FinanceTrackerApiClient",
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Secret"]?? throw new InvalidOperationException("JWT Secret not configured.")))
        };
    });

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

if (app.Environment.IsDevelopment())
{
    // Puedes proteger el UI con JWT si lo deseas:
    var docs = app.MapGroup("/docs").RequireAuthorization(); // <- elimina esta línea si lo quieres público
    docs.MapOpenApi();
    docs.MapScalarApiReference();
}

//app.UseHttpsRedirection();
app.MapDefaultEndpoints();
app.UseExceptionHandler();

app.UseAuthorization();

app.MapAuthEndpoints();
app.MapCategoryEndpoints();
app.MapTransactionEndpoints();
   
app.MigrateDatabase();
app.Run();
