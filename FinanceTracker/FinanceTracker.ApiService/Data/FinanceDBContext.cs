using System;
using FinanceTracker.ApiService.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.ApiService.Data;

public class FinanceDBContext(DbContextOptions<FinanceDBContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }

}
