namespace OrderDAL.Entity.Enums;

public enum OrderStatus
{
    Available = 0,
    Processing = 5,
    Reserved = 10,
    Cancelled = 15,
    Complete = 20,
}