namespace OrderBLL;

public static class GlobalConstants
{
    public const bool InProduction = false;
    
    public const string InventoryUrl = InProduction ? "http://inventory-service/api/item" : "http://localhost:5217/api/item";
}