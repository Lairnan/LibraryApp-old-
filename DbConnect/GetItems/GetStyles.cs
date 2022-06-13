using System.Collections;
using System.Data;
using DbConnect.Models;
using Npgsql;

namespace DbConnect.GetItems;

public class GetStyles : IEnumerable<Style>
{
    private readonly NpgsqlDataReader _dataReader;
    
    public GetStyles()
    {
        var dbConnection = new DbConnection();
        var npgsqlConnection = dbConnection.NpgsqlConnection;
        var state = dbConnection.StateTask;
        while (!state.IsCompleted) {}

        if (npgsqlConnection.State == ConnectionState.Closed)
            throw new NpgsqlException("Не удалось подключиться к базе данных");
        const string query = "SELECT s.id as id" +
                             ", s.name as name " +
                             "FROM styles s " +
                             "ORDER BY s.id";
        var npgsqlCommand = new NpgsqlCommand(query,npgsqlConnection);
        _dataReader = npgsqlCommand.ExecuteReader();
    }

    public IEnumerator<Style> GetEnumerator()
    {
        while (_dataReader.Read())
        {
            yield return new Style{
                Id = Convert.ToInt32(_dataReader["id"].ToString() ?? string.Empty),
                Name = _dataReader["name"].ToString() ?? string.Empty
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