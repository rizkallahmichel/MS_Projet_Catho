using MyApp.Domain.Products;

namespace MyApp.Application.Tests.Products;

public class ProductTests
{
    [Fact]
    public void DecreaseStock_ShouldReduceQuantity()
    {
        var product = Product.Create(Guid.NewGuid(), Guid.NewGuid(), "Vitamin C", null, 5m, 5);

        product.DecreaseStock(2);

        Assert.Equal(3, product.StockQuantity);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void DecreaseStock_ShouldThrowWhenQuantityIsNotPositive(int quantity)
    {
        var product = Product.Create(Guid.NewGuid(), Guid.NewGuid(), "Vitamin C", null, 5m, 5);

        Assert.Throws<ArgumentOutOfRangeException>(() => product.DecreaseStock(quantity));
    }

    [Fact]
    public void DecreaseStock_ShouldThrowWhenQuantityExceedsStock()
    {
        var product = Product.Create(Guid.NewGuid(), Guid.NewGuid(), "Vitamin C", null, 5m, 1);

        Assert.Throws<InvalidOperationException>(() => product.DecreaseStock(2));
    }
}
