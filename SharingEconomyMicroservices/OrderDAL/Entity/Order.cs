using OrderDAL.Entity.Enums;

namespace OrderDAL.Entity;

public class Order : BaseEntity
{
    public Order()
    {
        OrderStatus = OrderStatus.Available;
    }

    public int ItemId { get; set; }

    public string? Description { get; set; }

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    
    public virtual OrderStatus OrderStatus { get; set; }
}