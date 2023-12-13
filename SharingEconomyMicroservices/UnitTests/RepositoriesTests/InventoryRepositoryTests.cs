using InventoryDAL.Context;
using InventoryDAL.Entity;
using InventoryDAL.Entity.Enums;
using InventoryDAL.Repositories.Item;
using InventoryDAL.Repositories.Shared;
using Moq;
using UnitTests.Tool;

namespace UnitTests.RepositoriesTests;

public class InventoryRepositoryTests : TestWithInventoryPostgres<InventoryDbContext>
{
    private readonly ItemRepository _itemRepository;
    private readonly InventoryDbContext _context;

    public InventoryRepositoryTests()
    {
        _context = new InventoryDbContext(Options);
        _itemRepository = new ItemRepository(new GenericRepository<Item>(_context));
    }

    [Fact]
    public async Task Insert_Item_Should_Succeed()
    {
        await _context.Database.EnsureCreatedAsync();

        var item = await InsertItem();

        // Assert
        Assert.True(item.Id > 0);
    }


    [Fact]
    public async Task Get_Item_Should_Succeed()
    {
        await _context.Database.EnsureCreatedAsync();

        var item = await InsertItem();
        
        var result = await _itemRepository.GetById(item.Id);

        // Assert
        Assert.True(result != null && result.Id > 0);
    }

    [Fact]
    public async Task Update_Item_Should_Succeed()
    {
        await _context.Database.EnsureCreatedAsync();

        var item = await InsertItem();
        
        var updatedName = "UpdatedName";
        
        item.Name = updatedName;
        
        await _itemRepository.Update(item);
        
        var result = await _itemRepository.GetById(item.Id);

        // Assert
        Assert.Equal(result?.Name, updatedName);
    }

    [Fact] public async Task Delete_Item_Should_Succeed()
    {
        await _context.Database.EnsureCreatedAsync();

        var item = await InsertItem();
        
        var result = await _itemRepository.GetById(item.Id);
        
        Assert.NotNull(result);
        
        await _itemRepository.Delete(item);
        
        var deleted = await _itemRepository.GetById(item.Id);

        // Assert
        Assert.Null(deleted);
    }

    private async Task<Item> InsertItem()
    {
        var item = new Item()
        {
            Name = "Test",
            ItemType = ItemType.Scooter,
            Status = Status.Available
        };

        await _itemRepository.Insert(item);
        return item;
    }
}