using InventoryDAL.Entity;
using InventoryDAL.Entity.Enums;

namespace InventoryDAL.Helpers;

public static class ItemExtensions
{
    public static bool IsAvailable(this Item item)
    {
        return item.Status == Status.Available;
    }
}