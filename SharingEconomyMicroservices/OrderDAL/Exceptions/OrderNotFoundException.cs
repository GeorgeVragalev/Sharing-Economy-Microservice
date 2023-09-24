namespace OrderDAL.Exceptions;

public class OrderNotFoundException : Exception
{
    public OrderNotFoundException(string message) : base(message)
    {
        
    }
    public OrderNotFoundException() : base()
    {
        
    }
}