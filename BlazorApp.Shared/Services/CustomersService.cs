namespace BlazorApp.Shared.Services;

using BlazorApp.Shared.Dtos;
using BlazorApp.Shared.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

public interface ICustomerService
{
    Task AddAsync(Customer customer);
    Task<PagedResult<Customer>?> GetPagedAsync(int page, int pageSize);
    Task<Customer?> GetByIdAsync(string id);
    Task UpdateAsync(Customer customer);
    Task DeleteAsync(string id);
}

public class CustomerService : ICustomerService
{
    private readonly HttpClient _httpClient;
    private readonly ITokenService _tokenService;

    public CustomerService(HttpClient httpClient, ITokenService tokenService)
    {
        _httpClient = httpClient;
        _tokenService = tokenService;
        _httpClient.BaseAddress = new Uri("https://localhost:7018");
    }

    private async Task AddAuthorizationHeaderAsync()
    {
        var token = await _tokenService.GetTokenAsync();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task AddAsync(Customer customer)
    {
        await AddAuthorizationHeaderAsync();
        var response = await _httpClient.PostAsJsonAsync("/api/customers", customer);
        response.EnsureSuccessStatusCode();
    }

    public async Task<PagedResult<Customer>?> GetPagedAsync(int page, int pageSize)
    {
        await AddAuthorizationHeaderAsync();
        return await _httpClient.GetFromJsonAsync<PagedResult<Customer>>(
            $"/api/customers?page={page}&pageSize={pageSize}");
    }

    public async Task<Customer?> GetByIdAsync(string id)
    {
        await AddAuthorizationHeaderAsync();
        return await _httpClient.GetFromJsonAsync<Customer>($"/api/customers/{id}");
    }

    public async Task UpdateAsync(Customer customer)
    {
        await AddAuthorizationHeaderAsync();
        await _httpClient.PutAsJsonAsync($"/api/customers/{customer.Id}", customer);
    }

    public async Task DeleteAsync(string id)
    {
        await AddAuthorizationHeaderAsync();
        await _httpClient.DeleteAsync($"/api/customers/{id}");
    }
}

