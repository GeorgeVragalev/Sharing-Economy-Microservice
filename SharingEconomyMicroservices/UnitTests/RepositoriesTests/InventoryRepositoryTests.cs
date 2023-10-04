using InventoryDAL.Context;
using InventoryDAL.Entity;
using InventoryDAL.Entity.Enums;
using InventoryDAL.Repositories.Item;
using InventoryDAL.Repositories.Shared;
using UnitTests.Tool;

namespace UnitTests.RepositoriesTests;

public class InventoryRepositoryTests : TestWithPostgres<InventoryDbContext>
{
    private readonly ItemRepository _itemRepository;
    private readonly InventoryDbContext _context;

    public InventoryRepositoryTests()
    {
        _context = new InventoryDbContext(Options);
        _itemRepository = new ItemRepository(new GenericRepository<Item>(_context));
    }

    private Task SeedDatabaseThreeAccount()
    {
        
        return Task.CompletedTask;
    }

    [Fact]
    public async Task Insert_Item_Should_Succeed()
    {
        await _context.Database.EnsureCreatedAsync();

        var item = new Item()
        {
            Name = "Test",
            ItemType = ItemType.Scooter,
            Status = Status.Available
        };

        await _itemRepository.Insert(item);

        // Assert
        Assert.True(item.Id > 0);
    }
}