namespace OrderAPI.Models;

public class PlaceOrderRequestModel
{
    public int UserId { get; set; }
    public int ItemId { get; set; }
    public string? Description { get; set; }
    public TimeSpan ReservationPeriod { get; set; }
}