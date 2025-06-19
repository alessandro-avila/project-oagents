// EF Core query optimizations snippet demonstrating No-Tracking queries and projection
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

public class EfCoreQueryOptimizations
{
    private readonly AppDbContext _context;

    public EfCoreQueryOptimizations(AppDbContext context)
    {
        _context = context;
    }

    // Example: Use AsNoTracking for read-only queries to improve performance
    public async Task<List<CustomerDto>> GetActiveCustomersAsync()
    {
        // Project only required columns into DTO to reduce data transfer
        var customers = await _context.Customers
            .AsNoTracking() // Disable change tracking for better performance on read-only queries
            .Where(c => c.IsActive)
            .Select(c => new CustomerDto
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email
            })
            .ToListAsync();

        return customers;
    }
}

// Sample DTO class for projection
public class CustomerDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}

// Sample DbContext and entity classes for context - placeholders
public class AppDbContext : DbContext
{
    public DbSet<Customer> Customers { get; set; }
}

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public bool IsActive { get; set; }
}
