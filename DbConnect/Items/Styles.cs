using DbConnect.Models;
using Npgsql;

namespace DbConnect.Items;

public static class Styles
{
    private static void Check(NpgsqlConnection npgsqlConnection, string name)
    {
        var selCmd = new NpgsqlCommand("SELECT * FROM styles " +
                                       "WHERE lower(name) = @name ",
            npgsqlConnection);
        selCmd.Parameters.AddWithValue("name", name.ToLower());

        var dataReader = selCmd.ExecuteReader();

        try
        {
            if (dataReader.HasRows)
                throw new Exception("Данная запись уже существует в базе данных");
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
        finally
        {
            if(dataReader is {IsClosed:false})
                dataReader.Close();
        }
    }

    public static int Add(string name)
    {
        if (!DbConnection.IsConnected)
            DbConnection.Start();

        var npgsqlConnection = DbConnection.NpgsqlConnection;
        var state = DbConnection.IsConnected;
        if (!state)
        {
            throw new NpgsqlException("Не удалось подключиться к базе данных");
        }
        
        Check(npgsqlConnection,name);
        
        var insCmd =
            new NpgsqlCommand("INSERT INTO styles (name) VALUES (@name)",
                npgsqlConnection);
        insCmd.Parameters.AddWithValue("name", name);
        
        return insCmd.ExecuteNonQuery();
    }

    private static NpgsqlDataReader GetDataReader()
    {
        if (!DbConnection.IsConnected)
            DbConnection.Start();

        var npgsqlConnection = DbConnection.NpgsqlConnection;
        var state = DbConnection.IsConnected;
        if (!state)
        {
            throw new NpgsqlException("Не удалось подключиться к базе данных");
        }

        const string query = "SELECT s.id as id" +
                             ", s.name as name " +
                             "FROM styles s " +
                             "ORDER BY s.id";
        var npgsqlCommand = new NpgsqlCommand(query, npgsqlConnection);
        return npgsqlCommand.ExecuteReader();
    }

    public static IEnumerable<Style> GetItems()
    {
        var dataReader = GetDataReader();
        
        while (dataReader.Read())
        {
            yield return new Style{
                Id = Convert.ToInt32(dataReader["id"].ToString() ?? string.Empty),
                Name = dataReader["name"].ToString() ?? string.Empty
            };
        }

        if (dataReader is {IsClosed: false})
            dataReader.Close();
    }
}