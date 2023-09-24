using OrderAPI.Models;
using OrderDAL.Entity;

namespace OrderAPI.Helpers;

public static class OrderMapHelper
{
    public static Order MapOrder(this Order order,  PlaceOrderRequestModel requestModel)
    {
        order.StartTime = DateTime.UtcNow;
        order.EndTime = order.StartTime.Add(requestModel.ReservationPeriod);
        return order;
    } 
}