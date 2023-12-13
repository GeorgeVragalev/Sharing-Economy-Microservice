using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace UnitTests.Tool;

public abstract class TestWithOrderPostgres<T> : IDisposable where T : DbContext
{
    private const string ConnectionStringOrder = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=OrderAPI";
    private readonly NpgsqlConnection _connectionOrder;
    protected DbContextOptions<T> Options { get; }

    protected TestWithOrderPostgres()
    {
        var config = new ConfigurationBuilder().Build();

        _connectionOrder = new NpgsqlConnection(ConnectionStringOrder);
        _connectionOrder.Open();
        Options = new DbContextOptionsBuilder<T>().UseNpgsql(_connectionOrder).Options;
    }

    public void Dispose()
    {
        _connectionOrder.Close();
    }
}