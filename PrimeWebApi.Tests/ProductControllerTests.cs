using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PrimeWebApi.Controllers;
using PrimeWebApi.Models;

namespace PrimeWebApi.Tests;

public class ProductControllerTests
{
    private readonly Mock<ILogger<ProductController>> _mockLogger;
    private readonly ProductController _controller;

    public ProductControllerTests()
    {
        _mockLogger = new Mock<ILogger<ProductController>>();
        _controller = new ProductController(_mockLogger.Object);
    }

    [Fact]
    public void GetAllProducts_ReturnsAllProducts()
    {
        // Act
        var response = _controller.GetAllProducts();

        // Assert
        var products = Assert.IsAssignableFrom<IEnumerable<Product>>(response);
        Assert.Equal(10, products.Count());
    }

    [Fact]
    public void GetProduct_ValidId_ReturnsProduct()
    {
        // Arrange
        int validId = 1;

        // Act
        var response = _controller.GetProduct(validId);

        // Assert
        var okObject = Assert.IsType<OkObjectResult>(response.Result);
        var product = Assert.IsType<Product>(okObject.Value);
        
        Assert.Equal(validId, product.Id);
    }

    [Fact]
    public void GetProduct_InvalidId_ReturnsNotFound()
    {
        // Arrange
        int invalidId = 999;

        // Act
        var response = _controller.GetProduct(invalidId);

        // Assert
        Assert.IsType<NotFoundResult>(response.Result);
    }
}