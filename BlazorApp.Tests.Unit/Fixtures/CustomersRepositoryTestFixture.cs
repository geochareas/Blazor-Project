namespace BlazorApp.Tests.Unit.Fixtures;

using BlazorApp.Persistence;
using Microsoft.EntityFrameworkCore;

public class CustomerRepositoryTestFixture : IDisposable
{
    public ApplicationDbContext Context { get; private set; }

    public CustomerRepositoryTestFixture()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        Context = new ApplicationDbContext(options);

        // Optionally seed common test data
        Context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        Context.Dispose();
    }
}
