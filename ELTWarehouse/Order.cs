using ELTWarehouse.Enum;

namespace ELTWarehouse;

public class Order : BaseEntity
{
    public Order()
    {
        OrderStatus = OrderStatus.Available;
    }

    public int ItemId { get; set; }
    public int UserId { get; set; }

    public string? Description { get; set; }

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    
    public virtual OrderStatus OrderStatus { get; set; }
}