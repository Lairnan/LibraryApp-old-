using System.Data;
using DbConnect;
using Npgsql;

namespace ConsoleApp1;

public class Data
{
    private readonly NpgsqlDataReader _dataReader;

    public Data()
    {
        var dbConnection = new DBConnection();
        var npgsqlConnection = dbConnection.NpgsqlConnection;
        var state = dbConnection.StateTask;
        while (!state.IsCompleted) {}

        if (npgsqlConnection.State == ConnectionState.Closed)
            throw new NpgsqlException("Не удалось подключиться к базе данных");
        const string query = "SELECT * FROM books";
        var npgsqlCommand = new NpgsqlCommand(query,npgsqlConnection);
        _dataReader = npgsqlCommand.ExecuteReader();
    }

    public IEnumerable<NpgsqlDataReader> GetData()
    {
        while (_dataReader.Read())
        {
            yield return _dataReader;
        }

        if (_dataReader is {IsClosed: false})
            _dataReader.Close();
    }
}