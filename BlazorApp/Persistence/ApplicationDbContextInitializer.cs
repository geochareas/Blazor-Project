using BlazorApp.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp.Persistence;

public static class InitialiserExtensions
{
    public static void AddAsyncSeeding(this DbContextOptionsBuilder builder, IServiceProvider serviceProvider)
    {
        builder.UseAsyncSeeding(async (context, _, ct) =>
        {
            var initialiser = serviceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

            await initialiser.SeedAsync();
        });
    }

    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initialiser.InitialiseAsync();
    }
}

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;

    public ApplicationDbContextInitialiser(
        ILogger<ApplicationDbContextInitialiser> logger,
        ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            await _context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        if (await _context.Customers.AnyAsync()) return;

        var customers = new List<Customer>();
        var rand = new Random();

        for (int i = 1; i <= 50; i++)
        {
            var cityIndex = rand.Next(Cities.Length);
            customers.Add(new Customer
            {
                Id = i.ToString(),
                CompanyName = Companies[rand.Next(Companies.Length)],
                ContactName = $"Contact {i}",
                Address = $"Λεωφ. {rand.Next(100, 999)}",
                City = Cities[cityIndex],
                Region = Regions[cityIndex],
                PostalCode = $"{rand.Next(10000, 99999)}",
                Country = "Greece",
                Phone = $"+30 210 {rand.Next(1000000, 9999999)}"
            });
        }


        await _context.Customers.AddRangeAsync(customers);
        await _context.SaveChangesAsync();
    }

    private static readonly string[] Companies = { "Epsilon", "Microsoft", "Google" };
    private static readonly string[] Cities = { "Athens", "Thessaloniki", "Patras", "Heraklion" };
    private static readonly string[] Regions = { "Attica", "Central Macedonia", "Western Greece", "Crete" };
}
