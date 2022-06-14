using DbConnect.Models;
using Npgsql;

namespace DbConnect.Items;

public static class Loans
{
    public static int Add(int book, int reader, DateTime takenDate)
    {
        DbConnection.Start();

        var npgsqlConnection = DbConnection.NpgsqlConnection;
        var state = DbConnection.IsConnected;
        if (!state)
        {
            throw new NpgsqlException("Не удалось подключиться к базе данных");
        }

        var selCmd = new NpgsqlCommand("SELECT * FROM loans " +
                                       "WHERE book_id = @book_id " +
                                       "AND reader_id = @reader " +
                                       "AND passed = @passed ",
            npgsqlConnection);
        selCmd.Parameters.AddWithValue("book_id", book);
        selCmd.Parameters.AddWithValue("reader", reader);
        selCmd.Parameters.AddWithValue("passed", true);
        
        var dataReader = selCmd.ExecuteScalar();
        if (dataReader != null) throw new Exception("Данная запись уже существует в базе данных");

        var insCmd =
            new NpgsqlCommand("INSERT INTO loans (book_id, reader_id, taken_date, passed) VALUES (@book_id, @reader, @takenDate, @passed)",
                npgsqlConnection);
        insCmd.Parameters.AddWithValue("book_id", book);
        insCmd.Parameters.AddWithValue("reader", reader);
        insCmd.Parameters.AddWithValue("takenDate", takenDate);
        insCmd.Parameters.AddWithValue("passed", true);
        
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

        const string query = "SELECT l.id as id" +
                             ", b.name as book" +
                             ", concat_ws(' ',r.surname,r.name,r.patronymic) as reader" +
                             ", l.taken_date as taken_date" +
                             ", l.passed as passed " +
                             "FROM loans l " +
                             "LEFT JOIN books b on l.book_id = b.id " +
                             "LEFT JOIN readers r on l.reader_id = r.id " +
                             "ORDER BY l.id";
        var npgsqlCommand = new NpgsqlCommand(query, npgsqlConnection);
        return npgsqlCommand.ExecuteReader();
    }

    public static IEnumerable<Loan> GetItems()
    {
        var dataReader = GetDataReader();
        
        while (dataReader.Read())
        {
            DateTime.TryParse(dataReader["taken_date"].ToString(), out var takenDate);
            yield return new Loan{
                Id = Convert.ToInt32(dataReader["id"].ToString() ?? string.Empty),
                Book = dataReader["book"].ToString() ?? string.Empty,
                Reader = dataReader["reader"].ToString() ?? string.Empty,
                TakenDate = takenDate,
                Passed = bool.Parse(dataReader["passed"].ToString() ?? "false")
            };
        }

        if (dataReader is {IsClosed: false})
            dataReader.Close();
        
        DbConnection.Stop();
    }

    public static int Update(int id, int book, int reader, DateTime takenDate)
    {
        DbConnection.Start();

        var npgsqlConnection = DbConnection.NpgsqlConnection;
        var state = DbConnection.IsConnected;
        if (!state)
        {
            throw new NpgsqlException("Не удалось подключиться к базе данных");
        }
        
        var selCmd = new NpgsqlCommand("SELECT * FROM loans " +
                                       "WHERE id = @id",
            npgsqlConnection);
        selCmd.Parameters.AddWithValue("id", id);

        var data = selCmd.ExecuteScalar();
        if (data == null) throw new Exception("Выбранной записи не существует");
        
        
        var insCmd =
            new NpgsqlCommand("UPDATE loans SET book_id = @book, reader_id = @reader, taken_date = @takenDate WHERE id = @id",
                npgsqlConnection);
        insCmd.Parameters.AddWithValue("id", id);
        insCmd.Parameters.AddWithValue("book", book);
        insCmd.Parameters.AddWithValue("reader", reader);
        insCmd.Parameters.AddWithValue("takenDate", takenDate);

        var result = insCmd.ExecuteNonQuery();

        DbConnection.Stop();
        return result;
    }
}