using Npgsql;
using Type = DbConnect.Models.Type;

namespace DbConnect.Items;

public static class Types
{
    public static int Add(string name)
    {
        using var db = new DbConnection();
        var npgsqlConnection = db.NpgsqlConnection;
        var state = db.IsConnected;
        if (!state)
        {
            throw new NpgsqlException("Не удалось подключиться к базе данных");
        }
        
        var selCmd = new NpgsqlCommand("SELECT * FROM types " +
                                       "WHERE lower(name) = @name ",
            npgsqlConnection);
        selCmd.Parameters.AddWithValue("name", name.ToLower());
        
        var dataReader = selCmd.ExecuteScalar();
        if (dataReader != null) throw new Exception("Данная запись уже существует в базе данных");
        
        var insCmd =
            new NpgsqlCommand("INSERT INTO types (name) VALUES (@name)",
                npgsqlConnection);
        insCmd.Parameters.AddWithValue("name", name);
        
        var result = insCmd.ExecuteNonQuery();

        return result;
    }

    private static NpgsqlDataReader GetDataReader(DbConnection db)
    {
        var npgsqlConnection = db.NpgsqlConnection;
        var state = db.IsConnected;
        if (!state)
        {
            throw new NpgsqlException("Не удалось подключиться к базе данных");
        }

        const string query = "SELECT t.id as id" +
                             ", t.name as name " +
                             "FROM types t " +
                             "ORDER BY t.id";
        var npgsqlCommand = new NpgsqlCommand(query, npgsqlConnection);
        return npgsqlCommand.ExecuteReader();
    }

    public static IEnumerable<Type> Get()
    {
        using var db = new DbConnection();
        
        var dataReader = GetDataReader(db);
        
        while (dataReader.Read())
        {
            yield return new Type
            {
                Id = Convert.ToInt32(dataReader["id"].ToString() ?? string.Empty),
                Name = dataReader["name"].ToString() ?? string.Empty
            };
        }

        if (dataReader is {IsClosed: false})
            dataReader.Close();
    }

    public static int Update(int id, string name)
    {
        if (name == string.Empty) throw new Exception("Поля не должны быть пустыми");

        using var db = new DbConnection();
        var npgsqlConnection = db.NpgsqlConnection;
        var state = db.IsConnected;
        if (!state)
        {
            throw new NpgsqlException("Не удалось подключиться к базе данных");
        }
        
        var selCmd = new NpgsqlCommand("SELECT * FROM types " +
                                       "WHERE id = @id",
            npgsqlConnection);
        selCmd.Parameters.AddWithValue("id", id);

        var data = selCmd.ExecuteScalar();
        if (data == null) throw new Exception("Выбранной записи не существует");
        
        
        var insCmd =
            new NpgsqlCommand("UPDATE types SET name = @name WHERE id = @id",
                npgsqlConnection);
        insCmd.Parameters.AddWithValue("id", id);
        insCmd.Parameters.AddWithValue("name", name);

        var result = insCmd.ExecuteNonQuery();

        return result;
    }

    public static int Remove(int id)
    {
        using var db = new DbConnection();
        var npgsqlConnection = db.NpgsqlConnection;
        var state = db.IsConnected;
        if (!state)
        {
            throw new NpgsqlException("Не удалось подключиться к базе данных");
        }
        
        var selCmd = new NpgsqlCommand("SELECT * FROM types " +
                                       "WHERE id = @id",
            npgsqlConnection);
        selCmd.Parameters.AddWithValue("id", id);

        var data = selCmd.ExecuteScalar();
        if (data == null) throw new Exception("Выбранной записи не существует");

        var remCmd = new NpgsqlCommand("DELETE FROM types WHERE id = @id", npgsqlConnection);
        remCmd.Parameters.AddWithValue("id",id);

        return remCmd.ExecuteNonQuery();
    }
}