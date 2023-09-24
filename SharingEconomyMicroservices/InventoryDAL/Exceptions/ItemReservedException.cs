namespace InventoryDAL.Exceptions;

public class ItemReservedException : Exception
{
    public ItemReservedException(string message) : base(message)
    {
        
    }
}