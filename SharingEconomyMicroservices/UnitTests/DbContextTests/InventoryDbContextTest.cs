using InventoryDAL.Context;

namespace UnitTests.Tool;

public class InventoryDbContextTest : TestWithInventoryPostgres<InventoryDbContext>
{
    [Fact]
    public void CanConnect_ReturnTrue_AppDbContext()
    {
        using var context = new InventoryDbContext(Options);
        context.Database.EnsureCreated();

        var result = context.Database.CanConnect();

        Assert.True(result);
    }
}