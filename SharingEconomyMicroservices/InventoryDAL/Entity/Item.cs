using InventoryDAL.Entity.Enums;

namespace InventoryDAL.Entity;

public class Item : BaseEntity
{
    public string Name { get; set; }
    public virtual Status Status { get; set; }
    public virtual ItemType ItemType { get; set; }
}