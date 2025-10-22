using System;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.ApiService.Data;

public static class Dataextensions
{
    public static void MigrateDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FinanceDBContext>();
        db.Database.Migrate();
    }
}
