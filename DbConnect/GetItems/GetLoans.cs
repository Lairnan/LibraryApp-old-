using System.Collections;
using System.Data;
using DbConnect.Models;
using Npgsql;

namespace DbConnect.GetItems;

public class GetLoans : IEnumerable<Loan>
{
    private readonly NpgsqlDataReader _dataReader;

    public GetLoans()
    {
        var dbConnection = new DbConnection();
        var npgsqlConnection = dbConnection.NpgsqlConnection;
        var state = dbConnection.StateTask;
        while (!state.IsCompleted) {}

        if (npgsqlConnection.State == ConnectionState.Closed)
            throw new NpgsqlException("Не удалось подключиться к базе данных");
        const string query = "SELECT l.id as id" +
                             ", b.name as book" +
                             ", concat_ws(' ',r.surname,r.name,r.patronymic) as reader" +
                             ", l.taken_date as taken_date" +
                             ", l.passed as passed " +
                             "FROM loans l " +
                             "LEFT JOIN books b on l.book_id = b.id " +
                             "LEFT JOIN readers r on l.reader_id = r.id " +
                             "ORDER BY l.id";
        var npgsqlCommand = new NpgsqlCommand(query,npgsqlConnection);
        _dataReader = npgsqlCommand.ExecuteReader();
    }
    public IEnumerator<Loan> GetEnumerator()
    {
        while (_dataReader.Read())
        {
            DateTime.TryParse(_dataReader["taken_date"].ToString(), out var takenDate);
            yield return new Loan{
                Id = Convert.ToInt32(_dataReader["id"].ToString() ?? string.Empty),
                Book = _dataReader["book"].ToString() ?? string.Empty,
                Reader = _dataReader["reader"].ToString() ?? string.Empty,
                TakenDate = takenDate,
                Passed = bool.Parse(_dataReader["passed"].ToString() ?? "false")
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