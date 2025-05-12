using BlazorApp.Persistence.Configurations;
using BlazorApp.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace BlazorApp.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);

    }

    public DbSet<Customer> Customers { get; set; }
}
