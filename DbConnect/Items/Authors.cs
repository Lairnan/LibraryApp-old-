using System.Data;
using DbConnect.Models;
using Npgsql;

namespace DbConnect.Items;

public static class Authors
{
    public static int Add(string name, string surname,string? patronymic)
    {
        var npgsqlConnection = DbConnection.NpgsqlConnection;
        if (!DbConnection.IsConnected)
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

    private static NpgsqlDataReader GetDataReader()
    {
        var npgsqlConnection = DbConnection.NpgsqlConnection;
        if (!DbConnection.IsConnected)
        {
            throw new NpgsqlException("Не удалось подключиться к базе данных");
        }
        
        const string query = "SELECT a.id as id" +
                             ", a.name as name" +
                             ", a.surname as surname" +
                             ", a.patronymic as patronymic" +
                             " FROM authors a " +
                             " ORDER BY a.id";
        var npgsqlCommand = new NpgsqlCommand(query,npgsqlConnection);
        return npgsqlCommand.ExecuteReader();
    }

    private static NpgsqlDataReader GetDataReader(string name, string surname, string patronymic)
    {
        var npgsqlConnection = DbConnection.NpgsqlConnection;
        if (!DbConnection.IsConnected)
        {
            throw new NpgsqlException("Не удалось подключиться к базе данных");
        }

        const string query = "SELECT a.id as id" +
                             ", a.name as name" +
                             ", a.surname as surname" +
                             ", a.patronymic as patronymic" +
                             " FROM authors a" +
                             " WHERE LOWER(name) LIKE LOWER(@name)" +
                             " AND LOWER(surname) LIKE LOWER(@surname)" +
                             " AND LOWER(patronymic) LIKE LOWER(@patronymic)" +
                             " ORDER BY a.id";

        var npgsqlCommand = new NpgsqlCommand(query,npgsqlConnection);
        npgsqlCommand.Parameters.AddWithValue("name", $"%{name}%");
        npgsqlCommand.Parameters.AddWithValue("surname", $"%{surname}%");
        npgsqlCommand.Parameters.AddWithValue("patronymic", $"%{patronymic}%");
        return npgsqlCommand.ExecuteReader();
    }

    public static IEnumerable<Author> Search(string name = "", string surname = "", string patronymic = "")
    {
        if (!(!string.IsNullOrWhiteSpace(name)
              || !string.IsNullOrWhiteSpace(surname)
              || !string.IsNullOrWhiteSpace(patronymic)))
            throw new ArgumentException("Один из аргументов должен быть заполненным");

        var dataReader = GetDataReader(name, surname, patronymic);
        
        while (dataReader.Read())
        {
            yield return new Author{
                Id = Convert.ToInt32(dataReader["id"].ToString() ?? string.Empty),
                Name = dataReader["name"].ToString() ?? string.Empty,
                Surname = dataReader["surname"].ToString() ?? string.Empty,
                Patronymic = dataReader["patronymic"].ToString() != "" ? dataReader["patronymic"].ToString() : null
            };
        }

        if (dataReader is {IsClosed: false})
            dataReader.Close();
    }

    public static IEnumerable<Author> Get()
    {
        var dataReader = GetDataReader();
        
        while (dataReader.Read())
        {
            yield return new Author{
                Id = Convert.ToInt32(dataReader["id"].ToString() ?? string.Empty),
                Name = dataReader["name"].ToString() ?? string.Empty,
                Surname = dataReader["surname"].ToString() ?? string.Empty,
                Patronymic = dataReader["patronymic"].ToString() != "" ? dataReader["patronymic"].ToString() : null
            };
        }

        if (dataReader is {IsClosed: false})
            dataReader.Close();
    }

    public static int Update(int id,string name, string surname, string? patronymic)
    {
        if (name == string.Empty || surname == string.Empty) throw new Exception("Поля не должны быть пустыми");
        
        var npgsqlConnection = DbConnection.NpgsqlConnection;
        if (!DbConnection.IsConnected)
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
        var npgsqlConnection = DbConnection.NpgsqlConnection;
        if (!DbConnection.IsConnected)
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