using EcommerceApi.Models;

namespace EcommerceApi.Tests.Models;

public class ProductTests
{
    [Fact]
    public void Product_ShouldStoreNameAndPrice()
    {
        var product = new Product
        {
            Name = "Laptop",
            Price = 999.99m
        };

        var name = product.Name;
        var price = product.Price;

        Assert.Equal("Laptop", name);
        Assert.Equal(999.99m, price);
    }
}