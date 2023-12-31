﻿using System.Linq.Expressions;
using InventoryDAL.Entity.Enums;
using InventoryDAL.Exceptions;
using InventoryDAL.Helpers;
using InventoryDAL.Repositories.Shared;

namespace InventoryDAL.Repositories.Item;

public class ItemRepository : IItemRepository
{
    private readonly IGenericRepository<Entity.Item> _genericRepository;

    public ItemRepository(IGenericRepository<Entity.Item> genericRepository)
    {
        _genericRepository = genericRepository;
    }

    public async Task<Entity.Item?> GetById(int id)
    {
        return await _genericRepository.GetById(id);
    }

    public async Task<IQueryable<Entity.Item>> GetAll()
    {
        return await _genericRepository.GetAll();
    }

    public async Task<IQueryable<Entity.Item>> GetFiltered(Expression<Func<Entity.Item, bool>> filter)
    {
        return await _genericRepository.GetFiltered(filter);
    }

    public async Task Insert(Entity.Item item)
    {
        await _genericRepository.Insert(item);
    }

    public async Task Update(Entity.Item item)
    {
        await _genericRepository.Update(item);
    }

    public async Task Delete(Entity.Item item)
    {
        await _genericRepository.Delete(item);
    }

    public async Task<bool> DoesExist(Expression<Func<Entity.Item, bool>> filter)
    {
        return await _genericRepository.DoesExist(filter);
    }

    public async Task<bool> ChangeItemStatusAsync(int itemId, Status status)
    {
        return await _genericRepository.ExecuteInTransactionAsync(async () =>
        {
            var item = await _genericRepository.GetById(itemId);

            if (item == null)
            {
                throw new NotFoundException("Item doesn't exist.");
            }
            
            if (item.IsAvailable())
            {
                item.Status = status;
            }
            else
            {
                // Rollback will happen automatically if an exception is thrown
                throw new ItemReservedException("Item is not available for reservation.");
            }
        });
    }
}