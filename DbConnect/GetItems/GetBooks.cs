using System.Collections;
using System.Data;
using DbConnect.Models;
using Npgsql;

namespace DbConnect.GetItems;

public class GetBooks : IEnumerable<Book>
{
    private readonly NpgsqlDataReader _dataReader;
    
    public GetBooks()
    {
        var dbConnection = new DbConnection();
        var npgsqlConnection = dbConnection.NpgsqlConnection;
        var state = dbConnection.StateTask;
        while (!state.IsCompleted) {}

        if (npgsqlConnection.State == ConnectionState.Closed)
            throw new NpgsqlException("Не удалось подключиться к базе данных");
        const string query = "SELECT b.id as id" +
                             ", b.name as name" +
                             ", concat_ws(' ',a.name,a.surname,a.patronymic) as author" +
                             ", c.name as category" +
                             ", s.name as style " +
                             "FROM books b " +
                             "LEFT JOIN authors a on b.author_id = a.id " +
                             "LEFT JOIN categories c on b.categorie_id = c.id " +
                             "LEFT JOIN styles s on b.style_id = s.id " +
                             "ORDER BY b.id";
        var npgsqlCommand = new NpgsqlCommand(query,npgsqlConnection);
        _dataReader = npgsqlCommand.ExecuteReader();
    }

    public IEnumerator<Book> GetEnumerator()
    {
        while (_dataReader.Read())
        {
            yield return new Book{
                Id = Convert.ToInt32(_dataReader["id"].ToString() ?? string.Empty),
                Name = _dataReader["name"].ToString() ?? string.Empty,
                Author = _dataReader["author"].ToString() ?? string.Empty,
                Category = _dataReader["category"].ToString() ?? string.Empty,
                Style = _dataReader["style"].ToString() ?? string.Empty
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