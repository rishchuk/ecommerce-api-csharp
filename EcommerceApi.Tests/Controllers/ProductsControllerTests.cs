using System.Net;
using System.Net.Http.Json;
using EcommerceApi.DTOs;
using EcommerceApi.Models;
using EcommerceApi.Tests.Infrastructure;

namespace EcommerceApi.Tests.Controllers;

public class ProductsControllerTests : IClassFixture<PostgreSqlFixture>
{
    private readonly PostgreSqlFixture _postgres;

    public ProductsControllerTests(PostgreSqlFixture postgres)
    {
        _postgres = postgres;
    }

    [Fact]
    public async Task GetProducts_ShouldReturnOk()
    {
        await using var factory = new CustomWebApplicationFactory(_postgres.ConnectionString);

        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/products");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_ShouldReturnCreatedProduct()
    {
        await using var factory = new CustomWebApplicationFactory(_postgres.ConnectionString);

        using var client = factory.CreateClient();

        var product = new CreateProductDto
        {
            Name = "Test Laptop",
            Price = 999.99m
        };
        
        var response = await client.PostAsJsonAsync("/api/products", product);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdProduct = await response.Content.ReadFromJsonAsync<Product>();

        Assert.NotNull(createdProduct);
        Assert.True(createdProduct.Id > 0);
        Assert.Equal("Test Laptop", createdProduct.Name);
        Assert.Equal(999.99m, createdProduct.Price);
    }

    [Fact]
    public async Task CreateProduct_ShouldPersistProduct()
    {
        await using var factory = new CustomWebApplicationFactory(_postgres.ConnectionString);

        using var client = factory.CreateClient();

        var product = new CreateProductDto
        {
            Name = "Persistent Laptop",
            Price = 1499.99m
        };

        var createResponse = await client.PostAsJsonAsync("/api/products", product);

        var createdProduct = await createResponse.Content.ReadFromJsonAsync<Product>();
        
        var getResponse = await client.GetAsync($"/api/products/{createdProduct!.Id}");

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var savedProduct = await getResponse.Content.ReadFromJsonAsync<Product>();

        Assert.NotNull(savedProduct);
        Assert.Equal(createdProduct.Id, savedProduct.Id);
        Assert.Equal("Persistent Laptop", savedProduct.Name);
        Assert.Equal(1499.99m, savedProduct.Price);
    }
}