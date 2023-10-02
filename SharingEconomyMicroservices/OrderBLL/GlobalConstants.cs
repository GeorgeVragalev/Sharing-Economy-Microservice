namespace OrderBLL;

public static class GlobalConstants
{
    public const bool InProduction = true;
    
    public const string InventoryUrl = InProduction ? "http://inventory-service:80/api/item" : "http://localhost:5217/api/item";
}