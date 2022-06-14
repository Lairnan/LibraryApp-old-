using System.Collections;
using System.Data;
using DbConnect.Models;
using Npgsql;

namespace DbConnect.Items;

public static class Authors
{
    private static void Check(NpgsqlConnection npgsqlConnection, string name, string surname,string? patronymic)
    {
        var selCmd = new NpgsqlCommand("SELECT * FROM authors " +
                                       "WHERE lower(name) = @name " +
                                       "AND lower(surname) = @surname " +
                                       "AND lower(patronymic) = @patronymic",
            npgsqlConnection);
        selCmd.Parameters.AddWithValue("name", name.ToLower());
        selCmd.Parameters.AddWithValue("surname", surname.ToLower());
        selCmd.Parameters.AddWithValue("patronymic", patronymic?.ToLower() ?? string.Empty.ToLower());

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

    public static int Add(string name, string surname,string? patronymic)
    {
        DbConnection.Start();

        var npgsqlConnection = DbConnection.NpgsqlConnection;
        var state = DbConnection.IsConnected;
        if (!state)
        {
            throw new NpgsqlException("Не удалось подключиться к базе данных");
        }
        

        if (npgsqlConnection.State == ConnectionState.Closed)
            throw new NpgsqlException("Не удалось подключиться к базе данных");

        Check(npgsqlConnection,name,surname,patronymic);
        
        var insCmd =
            new NpgsqlCommand("INSERT INTO authors (name, surname, patronymic) VALUES (@name,@surname,@patronymic)",
                npgsqlConnection);
        insCmd.Parameters.AddWithValue("name", name);
        insCmd.Parameters.AddWithValue("surname", surname);
        insCmd.Parameters.AddWithValue("patronymic", patronymic ?? string.Empty);

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
        
        const string query = "SELECT a.id as id" +
                             ", a.name as name" +
                             ", a.surname as surname" +
                             ", a.patronymic as patronymic " +
                             "FROM authors a " +
                             "ORDER BY a.id";
        var npgsqlCommand = new NpgsqlCommand(query,npgsqlConnection);
        return npgsqlCommand.ExecuteReader();
    }

    public static IEnumerable<Author> GetItems()
    {
        var dataReader = GetDataReader();
        
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
        
        DbConnection.Stop();
    }
}