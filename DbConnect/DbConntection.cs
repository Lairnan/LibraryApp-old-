using Microsoft.Extensions.Configuration;
using Npgsql;

namespace DbConnect;

public static class DbConnection
{
    private static readonly object LockConnection = new();

    public static void Start()
    {
        lock (LockConnection)
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

    public static void Stop()
    {
        lock (LockConnection)
        {
            if (!IsConnected) return;
            NpgsqlConnection.Close();
            IsConnected = false;
        }
    }

    public static bool IsConnected { get; private set; }

    internal static NpgsqlConnection NpgsqlConnection = null!;
}