using System.Collections;
using System.Data;
using DbConnect.Models;
using Npgsql;

namespace DbConnect.GetItems;

public class GetReaders : IEnumerable<Reader>
{
    private readonly NpgsqlDataReader _dataReader;

    public GetReaders()
    {
        var dbConnection = new DbConnection();
        var npgsqlConnection = dbConnection.NpgsqlConnection;
        var state = dbConnection.StateTask;
        while (!state.IsCompleted) {}

        if (npgsqlConnection.State == ConnectionState.Closed)
            throw new NpgsqlException("Не удалось подключиться к базе данных");
        const string query = "SELECT r.id as id" +
                             ", r.name as name" +
                             ", r.surname as surname" +
                             ", r.patronymic as patronymic" +
                             ", r.birthday as birthday" +
                             ", t.name as typename " +
                             "FROM readers r " +
                             "LEFT JOIN types t on r.type_id = t.id " +
                             "ORDER BY r.id";
        var npgsqlCommand = new NpgsqlCommand(query,npgsqlConnection);
        _dataReader = npgsqlCommand.ExecuteReader();
    }

    public IEnumerator<Reader> GetEnumerator()
    {
        while (_dataReader.Read())
        {
            DateTime.TryParse(_dataReader["birthday"].ToString(), out var birthday);
            yield return new Reader{
                Id = Convert.ToInt32(_dataReader["id"].ToString() ?? string.Empty),
                Name = _dataReader["name"].ToString() ?? string.Empty,
                Surname = _dataReader["surname"].ToString() ?? string.Empty,
                Patronymic = _dataReader["patronymic"].ToString() ?? string.Empty,
                Birthday = birthday,
                Type = _dataReader["typename"].ToString() ?? string.Empty
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