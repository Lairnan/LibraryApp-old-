using DbConnect.Models;
using Npgsql;

namespace DbConnect.Items;

public static class Readers
{
    public static int Add(string name, string surname, string? patronymic, DateTime? birthday, int type)
    {
        var npgsqlConnection = DbConnection.NpgsqlConnection;
        var state = DbConnection.IsConnected;
        if (!state)
        {
            throw new NpgsqlException("Не удалось подключиться к базе данных");
        }

        var selCmd = new NpgsqlCommand("SELECT * FROM readers " +
                                       "WHERE lower(name) = @name " +
                                       "AND lower(surname) = @surname " +
                                       "AND lower(patronymic) = @patronymic",
            npgsqlConnection);
        selCmd.Parameters.AddWithValue("name", name.ToLower());
        selCmd.Parameters.AddWithValue("surname", surname.ToLower());
        selCmd.Parameters.AddWithValue("patronymic", patronymic?.ToLower() ?? string.Empty.ToLower());
        var dataReader = selCmd.ExecuteScalar();
        if (dataReader != null) throw new Exception("Данная запись уже существует в базе данных");
        
        var insCmd =
            new NpgsqlCommand("INSERT INTO readers (name, surname, patronymic, birthday, type_id) VALUES (@name,@surname,@patronymic,@birthday,@type_id)",
                npgsqlConnection);
        insCmd.Parameters.AddWithValue("name", name);
        insCmd.Parameters.AddWithValue("surname", surname);
        insCmd.Parameters.AddWithValue("patronymic", patronymic ?? string.Empty);
        insCmd.Parameters.AddWithValue("birthday", birthday!);
        insCmd.Parameters.AddWithValue("type_id", type);
        
        var result = insCmd.ExecuteNonQuery();

        return result;
    }

    private static NpgsqlDataReader GetDataReader()
    {
        var npgsqlConnection = DbConnection.NpgsqlConnection;
        var state = DbConnection.IsConnected;
        if (!state)
        {
            throw new NpgsqlException("Не удалось подключиться к базе данных");
        }

        const string query = "SELECT r.id as id" +
                             ", r.name as name" +
                             ", r.surname as surname" +
                             ", r.patronymic as patronymic" +
                             ", r.birthday as birthday" +
                             ", t.name as typename " +
                             "FROM readers r " +
                             "LEFT JOIN types t on r.type_id = t.id " +
                             "ORDER BY r.id";
        var npgsqlCommand = new NpgsqlCommand(query, npgsqlConnection);
        return npgsqlCommand.ExecuteReader();
    }

    public static IEnumerable<Reader> Get()
    {
        var dataReader = GetDataReader();
        
        while (dataReader.Read())
        {
            DateTime.TryParse(dataReader["birthday"].ToString(), out var birthday);
            yield return new Reader{
                Id = Convert.ToInt32(dataReader["id"].ToString() ?? string.Empty),
                Name = dataReader["name"].ToString() ?? string.Empty,
                Surname = dataReader["surname"].ToString() ?? string.Empty,
                Patronymic = dataReader["patronymic"].ToString() ?? string.Empty,
                Birthday = birthday,
                Type = dataReader["typename"].ToString() ?? string.Empty
            };
        }

        if (dataReader is {IsClosed: false})
            dataReader.Close();
    }

    public static int Update(int id, string name, string surname, string? patronymic, DateTime? birthday, int type)
    {
        if (name == string.Empty || surname == string.Empty) throw new Exception("Поля не должны быть пустыми");

        var npgsqlConnection = DbConnection.NpgsqlConnection;
        var state = DbConnection.IsConnected;
        if (!state)
        {
            throw new NpgsqlException("Не удалось подключиться к базе данных");
        }
        
        var selCmd = new NpgsqlCommand("SELECT * FROM readers " +
                                       "WHERE id = @id",
            npgsqlConnection);
        selCmd.Parameters.AddWithValue("id", id);

        var data = selCmd.ExecuteScalar();
        if (data == null) throw new Exception("Выбранной записи не существует");
        
        
        var insCmd =
            new NpgsqlCommand("UPDATE readers SET name = @name, surname = @surname, patronymic = @patronymic, birthday = @birthday, type_id = @type WHERE id = @id",
                npgsqlConnection);
        insCmd.Parameters.AddWithValue("id", id);
        insCmd.Parameters.AddWithValue("name", name);
        insCmd.Parameters.AddWithValue("surname", surname);
        insCmd.Parameters.AddWithValue("patronymic", patronymic ?? string.Empty);
        insCmd.Parameters.AddWithValue("birthday", birthday!);
        insCmd.Parameters.AddWithValue("type", type);

        var result = insCmd.ExecuteNonQuery();

        return result;
    }

    public static int Remove(int id)
    {
        var npgsqlConnection = DbConnection.NpgsqlConnection;
        var state = DbConnection.IsConnected;
        if (!state)
        {
            throw new NpgsqlException("Не удалось подключиться к базе данных");
        }
        
        var selCmd = new NpgsqlCommand("SELECT * FROM readers " +
                                       "WHERE id = @id",
            npgsqlConnection);
        selCmd.Parameters.AddWithValue("id", id);

        var data = selCmd.ExecuteScalar();
        if (data == null) throw new Exception("Выбранной записи не существует");

        var remCmd = new NpgsqlCommand("DELETE FROM readers WHERE id = @id", npgsqlConnection);
        remCmd.Parameters.AddWithValue("id",id);

        return remCmd.ExecuteNonQuery();
    }
}