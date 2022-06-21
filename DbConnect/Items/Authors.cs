using System.Data;
using DbConnect.Models;
using Npgsql;

namespace DbConnect.Items;

public static class Authors
{
    public static int Add(string name, string surname,string? patronymic)
    {
        using var db = new DbConnection();
        var npgsqlConnection = db.NpgsqlConnection;
        var state = db.IsConnected;
        if (!state)
        {
            throw new NpgsqlException("Не удалось подключиться к базе данных");
        }
        
        var selCmd = new NpgsqlCommand("SELECT * FROM authors " +
                                       "WHERE lower(name) = @name " +
                                       "AND lower(surname) = @surname " +
                                       "AND lower(patronymic) = @patronymic",
            npgsqlConnection);
        selCmd.Parameters.AddWithValue("name", name.ToLower());
        selCmd.Parameters.AddWithValue("surname", surname.ToLower());
        selCmd.Parameters.AddWithValue("patronymic", patronymic?.ToLower() ?? string.Empty.ToLower());
        
        var dataReader = selCmd.ExecuteScalar();
        if (dataReader != null) throw new Exception("Данная запись уже существует в базе данных");
        

        if (npgsqlConnection.State == ConnectionState.Closed)
            throw new NpgsqlException("Не удалось подключиться к базе данных");
        
        var insCmd =
            new NpgsqlCommand("INSERT INTO authors (name, surname, patronymic) VALUES (@name,@surname,@patronymic)",
                npgsqlConnection);
        insCmd.Parameters.AddWithValue("name", name);
        insCmd.Parameters.AddWithValue("surname", surname);
        insCmd.Parameters.AddWithValue("patronymic", patronymic ?? string.Empty);

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
        
        const string query = "SELECT a.id as id" +
                             ", a.name as name" +
                             ", a.surname as surname" +
                             ", a.patronymic as patronymic " +
                             "FROM authors a " +
                             "ORDER BY a.id";
        var npgsqlCommand = new NpgsqlCommand(query,npgsqlConnection);
        return npgsqlCommand.ExecuteReader();
    }

    public static IEnumerable<Author> Get()
    {
        using var db = new DbConnection();
        
        var dataReader = GetDataReader(db);
        
        while (dataReader.Read())
        {
            yield return new Author{
                Id = Convert.ToInt32(dataReader["id"].ToString() ?? string.Empty),
                Name = dataReader["name"].ToString() ?? string.Empty,
                Surname = dataReader["surname"].ToString() ?? string.Empty,
                Patronymic = dataReader["patronymic"].ToString() ?? string.Empty
            };
        }

        if (dataReader is {IsClosed: false})
            dataReader.Close();
    }

    public static int Update(int id,string name, string surname, string? patronymic)
    {
        if (name == string.Empty || surname == string.Empty) throw new Exception("Поля не должны быть пустыми");
        
        using var db = new DbConnection();
        var npgsqlConnection = db.NpgsqlConnection;
        var state = db.IsConnected;
        if (!state)
        {
            throw new NpgsqlException("Не удалось подключиться к базе данных");
        }
        
        var selCmd = new NpgsqlCommand("SELECT * FROM authors " +
                                       "WHERE id = @id",
            npgsqlConnection);
        selCmd.Parameters.AddWithValue("id", id);

        var data = selCmd.ExecuteScalar();
        if (data == null) throw new Exception("Выбранной записи не существует");
        
        
        var insCmd =
            new NpgsqlCommand("UPDATE authors SET name = @name, surname = @surname, patronymic = @patronymic WHERE id = @id",
                npgsqlConnection);
        insCmd.Parameters.AddWithValue("id", id);
        insCmd.Parameters.AddWithValue("name", name);
        insCmd.Parameters.AddWithValue("surname", surname);
        insCmd.Parameters.AddWithValue("patronymic", patronymic ?? string.Empty);

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
        
        var selCmd = new NpgsqlCommand("SELECT * FROM authors " +
                                       "WHERE id = @id",
            npgsqlConnection);
        selCmd.Parameters.AddWithValue("id", id);

        var data = selCmd.ExecuteScalar();
        if (data == null) throw new Exception("Выбранной записи не существует");

        var remCmd = new NpgsqlCommand("DELETE FROM authors WHERE id = @id", npgsqlConnection);
        remCmd.Parameters.AddWithValue("id",id);

        return remCmd.ExecuteNonQuery();
    }
}