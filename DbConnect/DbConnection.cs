using Microsoft.Extensions.Configuration;
using Npgsql;

namespace DbConnect;

public class DbConnection : IDisposable
{
    private readonly object _lockConnection = new();

    public DbConnection()
    {
        lock (_lockConnection)
        {
            if (IsConnected) return;
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build()
                .GetConnectionString("PostgresDB");
            NpgsqlConnection = new NpgsqlConnection(config);
            NpgsqlConnection.Open();
            IsConnected = true;
        }
    }
    
    public bool IsConnected { get; private set; }

    internal readonly NpgsqlConnection NpgsqlConnection = null!;

    public void Dispose()
    {
        lock (_lockConnection)
        {
            if (!IsConnected) return;
            NpgsqlConnection.Close();
            IsConnected = false;
        }
        
        GC.Collect();
    }
}