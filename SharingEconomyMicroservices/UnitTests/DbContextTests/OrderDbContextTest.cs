using OrderDAL.Context;

namespace UnitTests.Tool;

public class OrderDbContextTest : TestWithOrderPostgres<OrderDbContext>
{
    [Fact]
    public void CanConnect_ReturnTrue_AppDbContext()
    {
        using var context = new OrderDbContext(Options);
        context.Database.EnsureCreated();

        var result = context.Database.CanConnect();

        Assert.True(result);
    }
}