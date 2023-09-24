using InventoryDAL.Entity.Enums;

namespace InventoryAPI.Models;

public class ItemModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Status Status { get; set; }
    public ItemType ItemType { get; set; }
}