using BlazorApp.Shared.Dtos;
using BlazorApp.Shared.Models;

namespace BlazorApp.Interfaces;

public interface ICustomerRepository
{
    Task<PagedResult<Customer>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Customer?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task AddAsync(Customer customer, CancellationToken cancellationToken = default);
    Task UpdateAsync(Customer customer, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
}
