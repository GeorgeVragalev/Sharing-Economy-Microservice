using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace UnitTests.Tool;

public abstract class TestWithPostgres<T> : IDisposable where T : DbContext
{
    private const string ConnectionString = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=InventoryAPI";
    private readonly NpgsqlConnection _connection;
    protected DbContextOptions<T> Options { get; }

    protected TestWithPostgres()
    {
        _connection = new NpgsqlConnection(ConnectionString);
        _connection.Open();
        Options = new DbContextOptionsBuilder<T>().UseNpgsql(_connection).Options;
    }

    public void Dispose() => _connection.Close();
}