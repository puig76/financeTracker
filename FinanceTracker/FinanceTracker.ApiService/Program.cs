using FinanceTracker.ApiService.Data;
using FinanceTracker.ApiService.Services;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using FinanceTracker.ApiService.Controllers;
using FinanceTracker.ApiService.ExceptionsHandler;
using System.Diagnostics;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddProblemDetails(o =>
{
    o.CustomizeProblemDetails = (context) =>
    {
        context.ProblemDetails.Instance = $"{context.HttpContext.Request?.Method} {context.HttpContext.Request?.Path.Value}";
        context.ProblemDetails.Extensions.Add("requestId",context.HttpContext.TraceIdentifier);
        Activity? activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
        context.ProblemDetails.Extensions.Add("traceId1", activity?.Id);
    };
});
builder.Services.AddExceptionHandler<Global_ExceptionHandler>();

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();
builder.Services.AddAuthorization();



// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.AddNpgsqlDbContext<FinanceDBContext>("postgresdb");

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

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

//app.UseHttpsRedirection();
app.MapDefaultEndpoints();
app.UseExceptionHandler();

app.UseAuthorization();

app.MapAuthEndpoints();
   
app.MigrateDatabase();
app.Run();
