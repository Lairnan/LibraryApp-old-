using EncryptionSample;
using Microsoft.Win32;
using Npgsql;

namespace DbConnect;

public class DbConnection
{
    private static readonly object _lockConnection = new();
    private static string secretHash = "testSecret";

    public static void Connect()
    {
        lock (_lockConnection)
        {
            if (IsConnected) return;

            var key = Registry.CurrentUser.OpenSubKey("LibraryAPP");
            var host = key.GetValue("host").ToString();
            var user = key.GetValue("user").ToString();
            var pass = key.GetValue("password").ToString();
            var db = key.GetValue("database").ToString();
            var port = key.GetValue("port").ToString();
            
            NpgsqlConnection = new NpgsqlConnection($"host={host.Decrypt(secretHash)};" +
                                                    $"port={port};" +
                                                    $"User ID={user.Decrypt(secretHash)};" +
                                                    $"password={pass.Decrypt(secretHash)};" +
                                                    $"Database={db.Decrypt(secretHash)}");

            // var config = new ConfigurationBuilder()
            //     .SetBasePath(Directory.GetCurrentDirectory())
            //     .AddJsonFile("appsettings.json")
            //     .Build()
            //     .GetConnectionString("PostgresDB");
            // NpgsqlConnection = new NpgsqlConnection(config);
            
            NpgsqlConnection.Open();
            IsConnected = true;
        }
    }
    
    public static bool IsConnected { get; private set; }

    internal static NpgsqlConnection NpgsqlConnection = null!;

    public static void Close()
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