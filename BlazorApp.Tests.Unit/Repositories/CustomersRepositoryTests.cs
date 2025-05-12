namespace BlazorApp.Tests.Unit.Repositories;

using BlazorApp.Persistence.Repositories;
using BlazorApp.Shared.Models;
using BlazorApp.Tests.Unit.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class CustomerRepositoryTests : IClassFixture<CustomerRepositoryTestFixture>
{
    private readonly CustomerRepositoryTestFixture _fixture;
    private readonly CustomerRepository _repository;

    public CustomerRepositoryTests(CustomerRepositoryTestFixture fixture)
    {
        _fixture = fixture;

        // Clean up before each test
        _fixture.Context.Customers.RemoveRange(_fixture.Context.Customers);
        _fixture.Context.SaveChanges();

        _repository = new CustomerRepository(_fixture.Context);
    }

    [Fact]
    public async Task AddAsync_ShouldAddCustomer()
    {
        var customer = new Customer { Id = "C123", CompanyName = "Test" };

        await _repository.AddAsync(customer);

        var found = await _fixture.Context.Customers.FindAsync("C123");
        found.Should().NotBeNull();
    }

    [Fact]
    public async Task GetPagedAsync_ShouldReturnPagedList()
    {
        var customers = Enumerable.Range(1, 15).Select(i =>
            new Customer { Id = $"C{i:D3}", CompanyName = $"Company {i}" });

        await _fixture.Context.Customers.AddRangeAsync(customers);
        await _fixture.Context.SaveChangesAsync();

        var result = await _repository.GetPagedAsync(page: 2, pageSize: 5);

        result.Items.Should().HaveCount(5);
        result.Page.Should().Be(2);
        result.TotalCount.Should().Be(15);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCustomer_WhenExists()
    {
        var customer = new Customer { Id = "C001", CompanyName = "Test" };
        await _fixture.Context.Customers.AddAsync(customer);
        await _fixture.Context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync("C001");

        result.Should().NotBeNull();
        result!.CompanyName.Should().Be("Test");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        var result = await _repository.GetByIdAsync("NonExistent");

        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateCustomer()
    {
        var customer = new Customer { Id = "C002", CompanyName = "BeforeUpdate" };
        await _fixture.Context.Customers.AddAsync(customer);
        await _fixture.Context.SaveChangesAsync();

        customer.CompanyName = "AfterUpdate";
        await _repository.UpdateAsync(customer);

        var updated = await _fixture.Context.Customers.FindAsync("C002");
        updated!.CompanyName.Should().Be("AfterUpdate");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveCustomer_WhenExists()
    {
        var customer = new Customer { Id = "C003", CompanyName = "ToDelete" };
        await _fixture.Context.Customers.AddAsync(customer);
        await _fixture.Context.SaveChangesAsync();

        await _repository.DeleteAsync("C003");

        var exists = await _fixture.Context.Customers.FindAsync("C003");
        exists.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_ShouldDoNothing_WhenCustomerNotFound()
    {
        // Just verify that no exception is thrown
        await _repository.DeleteAsync("NonExistent");

        // Context remains unchanged
        var count = await _fixture.Context.Customers.CountAsync();
        count.Should().Be(0);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldReturnEmpty_WhenPageOutOfRange()
    {
        var customers = Enumerable.Range(1, 5).Select(i =>
            new Customer { Id = $"C{i}", CompanyName = $"Company {i}" });

        await _fixture.Context.Customers.AddRangeAsync(customers);
        await _fixture.Context.SaveChangesAsync();

        var result = await _repository.GetPagedAsync(page: 2, pageSize: 10);

        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(5);
    }

}
