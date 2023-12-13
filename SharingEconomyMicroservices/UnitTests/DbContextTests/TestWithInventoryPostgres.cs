using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace UnitTests.Tool;

public abstract class TestWithInventoryPostgres<T> : IDisposable where T : DbContext
{
    private const string ConnectionStringInventory = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=InventoryAPI";
    private readonly NpgsqlConnection _connectionInventory;
    protected DbContextOptions<T> Options { get; }

    protected TestWithInventoryPostgres()
    {
        var config = new ConfigurationBuilder().Build();

        _connectionInventory = new NpgsqlConnection(ConnectionStringInventory);
        _connectionInventory.Open();
        Options = new DbContextOptionsBuilder<T>().UseNpgsql(_connectionInventory).Options;
    }

    public void Dispose()
    {
        _connectionInventory.Close();
    }
}