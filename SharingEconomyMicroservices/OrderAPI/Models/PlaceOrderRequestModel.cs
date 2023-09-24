using OrderDAL.Entity.Enums;

namespace OrderAPI.Models;

public class PlaceOrderRequestModel
{
    public int ItemId { get; set; }
    public string? Description { get; set; }
    public TimeSpan ReservationPeriod { get; set; }
}