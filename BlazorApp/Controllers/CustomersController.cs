using BlazorApp.Interfaces;
using BlazorApp.Shared.Dtos;
using BlazorApp.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerRepository _repository;
    private readonly ILogger<CustomersController> _logger;

    public CustomersController(ICustomerRepository repository, ILogger<CustomersController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<Customer>>> Get(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (page <= 0 || pageSize <= 0)
        {
            return Problem(
                title: "Invalid pagination parameters",
                detail: "Page and pageSize must be greater than zero.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        var result = await _repository.GetPagedAsync(page, pageSize, cancellationToken);

        return Ok(result);
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<Customer>> GetById(string id, CancellationToken cancellationToken)
    {
        var customer = await _repository.GetByIdAsync(id, cancellationToken);
        if (customer is null)
        {
            return Problem(
                title: "Customer Not Found",
                detail: $"Customer with ID '{id}' was not found.",
                statusCode: StatusCodes.Status404NotFound);
        }

        return Ok(customer);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Customer customer, CancellationToken cancellationToken)
    {
        if (customer is null)
        {
            return Problem(
                title: "Invalid Request",
                detail: "Customer cannot be null.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        if (string.IsNullOrWhiteSpace(customer.Id))
        {
            customer.Id = Guid.NewGuid().ToString();
        }

        await _repository.AddAsync(customer, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] Customer customer, CancellationToken cancellationToken)
    {
        if (id != customer.Id)
        {
            return Problem(
                title: "ID Mismatch",
                detail: "ID mismatch between route and body.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        await _repository.UpdateAsync(customer, cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        await _repository.DeleteAsync(id, cancellationToken);

        return NoContent();
    }
}
