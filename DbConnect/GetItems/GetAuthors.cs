using System.Collections;
using System.Data;
using DbConnect.Models;
using Npgsql;

namespace DbConnect.GetItems;

public class GetAuthors : IEnumerable<Author>
{
    private readonly NpgsqlDataReader _dataReader;
    
    public GetAuthors()
    {
        var dbConnection = new DbConnection();
        var npgsqlConnection = dbConnection.NpgsqlConnection;
        var state = dbConnection.StateTask;
        while (!state.IsCompleted) {}

        if (npgsqlConnection.State == ConnectionState.Closed)
            throw new NpgsqlException("Не удалось подключиться к базе данных");
        const string query = "SELECT a.id as id" +
                             ", a.name as name" +
                             ", a.surname as surname" +
                             ", a.patronymic as patronymic " +
                             "FROM authors a " +
                             "ORDER BY a.id";
        var npgsqlCommand = new NpgsqlCommand(query,npgsqlConnection);
        _dataReader = npgsqlCommand.ExecuteReader();
    }

    public IEnumerator<Author> GetEnumerator()
    {
        while (_dataReader.Read())
        {
            yield return new Author{
                Id = Convert.ToInt32(_dataReader["id"].ToString() ?? string.Empty),
                Name = _dataReader["name"].ToString() ?? string.Empty,
                Surname = _dataReader["surname"].ToString() ?? string.Empty,
                Patronymic = _dataReader["patronymic"].ToString() ?? string.Empty
            };
        }

        if (_dataReader is {IsClosed: false})
            _dataReader.Close();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}