using System.Collections;
using System.Data;
using Npgsql;
using Type = DbConnect.Models.Type;

namespace DbConnect.GetItems;

public class GetTypes : IEnumerable<Type>
{
    private readonly NpgsqlDataReader _dataReader;
    
    public GetTypes()
    {
        var dbConnection = new DbConnection();
        var npgsqlConnection = dbConnection.NpgsqlConnection;
        var state = dbConnection.StateTask;
        while (!state.IsCompleted) {}

        if (npgsqlConnection.State == ConnectionState.Closed)
            throw new NpgsqlException("Не удалось подключиться к базе данных");
        const string query = "SELECT t.id as id" +
                             ", t.name as name " +
                             "FROM types t " +
                             "ORDER BY t.id";
        var npgsqlCommand = new NpgsqlCommand(query,npgsqlConnection);
        _dataReader = npgsqlCommand.ExecuteReader();
    }

    public IEnumerator<Type> GetEnumerator()
    {
        while (_dataReader.Read())
        {
            yield return new Type
            {
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