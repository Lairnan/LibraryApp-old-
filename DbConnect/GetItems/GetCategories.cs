using System.Collections;
using System.Data;
using DbConnect.Models;
using Npgsql;

namespace DbConnect.GetItems;

public class GetCategories : IEnumerable<Category>
{
    private readonly NpgsqlDataReader _dataReader;
    
    public GetCategories()
    {
        var dbConnection = new DbConnection();
        var npgsqlConnection = dbConnection.NpgsqlConnection;
        var state = dbConnection.StateTask;
        while (!state.IsCompleted) {}

        if (npgsqlConnection.State == ConnectionState.Closed)
            throw new NpgsqlException("Не удалось подключиться к базе данных");
        const string query = "SELECT c.id as id" +
                             ", c.name as name " +
                             "FROM categories c " +
                             "ORDER BY c.id";
        var npgsqlCommand = new NpgsqlCommand(query,npgsqlConnection);
        _dataReader = npgsqlCommand.ExecuteReader();
    }

    public IEnumerator<Category> GetEnumerator()
    {
        while (_dataReader.Read())
        {
            yield return new Category{
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