using OrderDAL.Entity.Enums;

namespace OrderAPI.Models;

public class OrderModel
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public string? Description { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public virtual OrderStatus OrderStatus { get; set; }
}