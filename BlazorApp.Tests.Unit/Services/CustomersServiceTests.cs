using BlazorApp.Shared.Dtos;
using BlazorApp.Shared.Models;
using BlazorApp.Shared.Services;
using FluentAssertions;
using NSubstitute;
using System.Net;
using System.Net.Http.Json;

namespace BlazorApp.Tests.Unit.Services;

public class CustomerServiceTests
{
    private readonly ITokenService _tokenService;

    public CustomerServiceTests()
    {
        _tokenService = Substitute.For<ITokenService>();
        _tokenService.GetTokenAsync().Returns("fake-token");
    }

    [Fact]
    public async Task GetPagedAsync_ShouldReturnPagedResult_WhenApiReturnsSuccess()
    {
        // Arrange
        var expectedPagedResult = new PagedResult<Customer>
        {
            Items = [new Customer { Id = "1", CompanyName = "George Chareas" }],
            TotalCount = 1
        };

        var mockResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonContent.Create(expectedPagedResult)
        };

        var mockHandler = new MockHttpMessageHandler(mockResponse);
        var httpClient = new HttpClient(mockHandler) { BaseAddress = new Uri("https://localhost:7018") };

        var customerService = new CustomerService(httpClient, _tokenService);

        // Act
        var result = await customerService.GetPagedAsync(1, 10);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedPagedResult);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldThrowException_WhenApiReturnsError()
    {
        // Arrange
        var mockResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.InternalServerError
        };

        var mockHandler = new MockHttpMessageHandler(mockResponse);
        var httpClient = new HttpClient(mockHandler) { BaseAddress = new Uri("https://localhost:7018") };

        var customerService = new CustomerService(httpClient, _tokenService);

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => customerService.GetPagedAsync(1, 10));
    }

    [Fact]
    public async Task GetPagedAsync_ShouldReturnEmptyList_WhenApiReturnsEmptyList()
    {
        // Arrange
        var expectedPagedResult = new PagedResult<Customer> { Items = [], TotalCount = 0 };

        var mockResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonContent.Create(expectedPagedResult)
        };

        var mockHandler = new MockHttpMessageHandler(mockResponse);
        var httpClient = new HttpClient(mockHandler) { BaseAddress = new Uri("https://localhost:7018") };

        var customerService = new CustomerService(httpClient, _tokenService);

        // Act
        var result = await customerService.GetPagedAsync(1, 10);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }
}
