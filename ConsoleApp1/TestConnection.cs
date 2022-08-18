using DbConnect;
using EncryptionSample;
using Microsoft.Win32;

namespace ConsoleApp1;

public class TestConnection
{
    private static string secretHash = "testSecret";

    static TestConnection()
    {
        while (true)
        {
            try
            {
                DbConnection.Connect();
                Console.WriteLine(DbConnection.IsConnected);
                break;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                var key = Registry.CurrentUser.CreateSubKey("LibraryAPP");
                var host = Console.ReadLine();
                var user = Console.ReadLine();
                var password = Console.ReadLine();
                var database = Console.ReadLine();
                var port = Console.ReadLine();
                key.SetValue("host", host.Encrypt(secretHash));
                key.SetValue("user", user.Encrypt(secretHash));
                key.SetValue("password", password.Encrypt(secretHash));
                key.SetValue("database", database.Encrypt(secretHash));
                key.SetValue("port", port);
            }
            finally
            {
                DbConnection.Close();
            }
        }
    }
}