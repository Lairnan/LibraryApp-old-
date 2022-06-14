using DbConnect.Models;
using Npgsql;

namespace DbConnect.Items;

public static class Categories
{
    private static void Check(NpgsqlConnection npgsqlConnection, string name)
    {
        var selCmd = new NpgsqlCommand("SELECT * FROM categories " +
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
        DbConnection.Start();
        
        var npgsqlConnection = DbConnection.NpgsqlConnection;
        var state = DbConnection.IsConnected;
        if (!state)
        {
            throw new NpgsqlException("Не удалось подключиться к базе данных");
        }

        Check(npgsqlConnection,name);
        
        var insCmd =
            new NpgsqlCommand("INSERT INTO categories (name) VALUES (@name)",
                npgsqlConnection);
        insCmd.Parameters.AddWithValue("name", name);
        
        var result = insCmd.ExecuteNonQuery();

        DbConnection.Stop();
        return result;
    }

    private static NpgsqlDataReader GetDataReader()
    {
        DbConnection.Start();

        var npgsqlConnection = DbConnection.NpgsqlConnection;
        var state = DbConnection.IsConnected;
        if (!state)
        {
            throw new NpgsqlException("Не удалось подключиться к базе данных");
        }

        const string query = "SELECT c.id as id" +
                             ", c.name as name " +
                             "FROM categories c " +
                             "ORDER BY c.id";
        var npgsqlCommand = new NpgsqlCommand(query, npgsqlConnection);
        return npgsqlCommand.ExecuteReader();
    }

    public static IEnumerable<Category> GetItems()
    {
        var dataReader = GetDataReader();
        
        while (dataReader.Read())
        {
            yield return new Category{
                Id = Convert.ToInt32(dataReader["id"].ToString() ?? string.Empty),
                Name = dataReader["name"].ToString() ?? string.Empty
            };
        }

        if (dataReader is {IsClosed: false})
            dataReader.Close();
        
        DbConnection.Stop();
    }
}