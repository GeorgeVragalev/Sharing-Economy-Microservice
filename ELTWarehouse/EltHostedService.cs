using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Npgsql;

namespace ELTWarehouse;

public class EltHostedService : BackgroundService
{
    private readonly IConfiguration _configuration;

    public EltHostedService(IConfiguration configuration)
    {
        _configuration = configuration;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Call your ELT process method here
            await RunEltProcessAsync();

            // Wait for the next run (e.g., every 24 hours)
            await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
        }
    }

    public async Task RunEltProcessAsync()
    {
        var inventoryConnectionString = _configuration.GetConnectionString("InventoryDb");
        var orderConnectionString = _configuration.GetConnectionString("OrderDb");
        var warehouseConnectionString = _configuration.GetConnectionString("WarehouseDb");


        var inventoryConnection = new DatabaseConnection("YourInventoryDbConnectionString");
        var orderConnection = new DatabaseConnection("YourOrderDbConnectionString");
        var warehouseConnection = new DatabaseConnection("YourWarehouseConnectionString");

        var extractedItems = await ExtractItemsAsync(inventoryConnection.CreateConnection());
        await LoadItemsAsync(extractedItems, warehouseConnection.CreateConnection());

        var extractedOrders = await ExtractOrdersAsync(orderConnection.CreateConnection());
        await LoadOrdersAsync(extractedOrders, warehouseConnection.CreateConnection());

        // Include any transformation logic if necessary
    }

    public async Task LoadItemsAsync(IEnumerable<Item> items, IDbConnection connection)
    {
        // Open connection
        // Iterate over items and insert them into the DW_Items table
        // Use parameters to prevent SQL injection
    }

    public async Task<IEnumerable<Item>> ExtractItemsAsync(IDbConnection connection)
    {
        var items = new List<Item>();
        var query = "SELECT * FROM Items"; // Adjust the query based on your actual table

        // Rest of the extraction code similar to the previous example
        // Map the data from the reader to the Item entity
    }

    public async Task LoadDataAsync(IEnumerable<MyDataModel> data, IDbConnection connection)
    {
        await connection.OpenAsync();

        // Example SQL command to insert data
        var insertCommand = "INSERT INTO WarehouseTable (Column1, Column2) VALUES (@col1, @col2)";

        foreach (var item in data)
        {
            await using (var cmd = new NpgsqlCommand(insertCommand, (NpgsqlConnection)connection))
            {
                cmd.Parameters.AddWithValue("@col1", item.Property1);
                cmd.Parameters.AddWithValue("@col2", item.Property2);
                // ... other parameters ...

                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
    
    public async Task LoadOrdersAsync(IEnumerable<Order> orders, IDbConnection connection)
    {
        connection.Open();

        var insertCommand = "INSERT INTO DW_Orders (OrderId, ItemId, UserId, Description, StartTime, EndTime, OrderStatus) VALUES (@orderId, @itemId, @userId, @description, @startTime, @endTime, @orderStatus)";

        foreach (var order in orders)
        {
            await using (var cmd = new NpgsqlCommand(insertCommand, (NpgsqlConnection)connection))
            {
                // Assuming OrderId, ItemId, etc. are properties of your Order class
                cmd.Parameters.AddWithValue("@orderId", order.OrderId);
                cmd.Parameters.AddWithValue("@itemId", order.ItemId);
                // ... other parameters ...

                await cmd.ExecuteNonQueryAsync();
            }
        }
    }


    public async Task<IEnumerable<Order>> ExtractNewOrdersAsync(IDbConnection connection, DateTime lastRunDate)
    {
        var newOrders = new List<Order>();
        var query = "SELECT * FROM Orders WHERE LastUpdatedAt > @lastRunDate";

        await using (var cmd = new NpgsqlCommand(query, (NpgsqlConnection)connection))
        {
            cmd.Parameters.AddWithValue("@lastRunDate", lastRunDate);

            connection.Open();
            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    newOrders.Add(new Order
                    {
                        // Map data from reader to Order entity
                    });
                }
            }
        }

        return newOrders;
    }
}