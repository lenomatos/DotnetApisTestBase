using Microsoft.AspNetCore.Mvc;
using PrimeWebApi.Models;

namespace PrimeWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private readonly List<Product> products = new()
    {
        new Product { Id = 1, Name = "Laptop", Price = 999.99m },
        new Product { Id = 2, Name = "Smartphone", Price = 699.99m },
        new Product { Id = 3, Name = "Headphones", Price = 149.99m },
        new Product { Id = 4, Name = "Keyboard", Price = 49.99m },
        new Product { Id = 5, Name = "Mouse", Price = 19.99m },
        new Product { Id = 6, Name = "Monitor", Price = 199.99m },
        new Product { Id = 7, Name = "USB Cable", Price = 9.99m },
        new Product { Id = 8, Name = "Notebook", Price = 4.99m },
        new Product { Id = 9, Name = "Coffee Mug", Price = 12.99m },
        new Product { Id = 10, Name = "Backpack", Price = 39.99m }
    };

    private readonly ILogger<ProductController> _logger;

    public ProductController(ILogger<ProductController> logger)
    {
        _logger = logger;
    }

    [HttpGet("all")]
    public IEnumerable<Product> GetAllProducts()
    {
        return products;
    }

    [HttpGet("{id}")]
    public ActionResult<Product> GetProduct(int id)
    {
        var product = products.FirstOrDefault(p => p.Id == id);
        return product == null ? NotFound() : Ok(product);
    }


}