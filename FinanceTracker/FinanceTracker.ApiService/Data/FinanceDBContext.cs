using System;
using FinanceTracker.ApiService.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.ApiService.Data;

public class FinanceDBContext(DbContextOptions<FinanceDBContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }

    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Food", Description = "Expenses related to food and groceries." },
            new Category { Id = 2, Name = "Transportation", Description = "Expenses related to transportation and travel." },
            new Category { Id = 3, Name = "Utilities", Description = "Expenses related to utilities such as electricity, water, and internet." },
            new Category { Id = 4, Name = "Entertainment", Description = "Expenses related to entertainment and leisure activities." },
            new Category { Id = 5, Name = "Healthcare", Description = "Expenses related to medical and healthcare services." }
        );
    }
}
