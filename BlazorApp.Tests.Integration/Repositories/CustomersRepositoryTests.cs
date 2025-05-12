using BlazorApp.Persistence.Repositories;
using BlazorApp.Shared.Models;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorApp.Tests.Integration.Repositories;

public class CustomerRepositoryIntegrationTests
{
    [Fact]
    public async Task AddAsync_ShouldPersistCustomer()
    {
        // Arrange
        var context = TestDbContextFactory.CreateInMemoryContext("AddAsyncTest");
        var repository = new CustomerRepository(context);

        var customer = new Customer { Id = "1", CompanyName = "Company" };

        // Act
        await repository.AddAsync(customer);

        // Assert
        var saved = await context.Customers.FindAsync("1");
        saved.Should().NotBeNull();
        saved!.CompanyName.Should().Be("Company");
    }

    [Fact]
    public async Task GetPagedAsync_ShouldReturnPagedResults()
    {
        // Arrange
        var context = TestDbContextFactory.CreateInMemoryContext("PagedTest");
        var repository = new CustomerRepository(context);

        for (int i = 1; i <= 25; i++)
        {
            context.Customers.Add(new Customer { Id = i.ToString(), CompanyName = $"Company {i}" });
        }
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetPagedAsync(2, 10); // page 2, page size 10

        // Assert
        result.Items.Should().HaveCount(10);
        result.Page.Should().Be(2);
        result.TotalCount.Should().Be(25);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveCustomer()
    {
        var context = TestDbContextFactory.CreateInMemoryContext("DeleteTest");
        var repository = new CustomerRepository(context);

        var customer = new Customer { Id = "99", CompanyName = "To Be Deleted" };
        context.Customers.Add(customer);
        await context.SaveChangesAsync();

        // Act
        await repository.DeleteAsync("99");

        // Assert
        var deleted = await context.Customers.FindAsync("99");
        deleted.Should().BeNull();
    }
}
