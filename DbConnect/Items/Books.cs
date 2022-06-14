using DbConnect.Models;
using Npgsql;

namespace DbConnect.Items;

public static class Books
{
    private static void Check(NpgsqlConnection npgsqlConnection, string name, int author)
    {
        var selCmd = new NpgsqlCommand("SELECT * FROM books " +
                                       "WHERE lower(name) = @name " +
                                       "AND author_id = @author_id",
            npgsqlConnection);
        selCmd.Parameters.AddWithValue("name", name.ToLower());
        selCmd.Parameters.AddWithValue("author_id", author);

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

    public static int Add(string name, int author, int category, int style)
    {
        if (!DbConnection.IsConnected)
            DbConnection.Start();
        
        var npgsqlConnection = DbConnection.NpgsqlConnection;
        var state = DbConnection.IsConnected;
        if (!state)
        {
            throw new NpgsqlException("Не удалось подключиться к базе данных");
        }

        Check(npgsqlConnection,name,author);
        
        var insCmd =
            new NpgsqlCommand("INSERT INTO books (name, author_id, categorie_id, style_id) VALUES (@name,@author_id,@categorie_id,@style_id)",
                npgsqlConnection);
        insCmd.Parameters.AddWithValue("name", name);
        insCmd.Parameters.AddWithValue("author_id", author);
        insCmd.Parameters.AddWithValue("categorie_id", category);
        insCmd.Parameters.AddWithValue("style_id", style);
        
        return insCmd.ExecuteNonQuery();
    }

    private static NpgsqlDataReader GetDataReader()
    {
        if (!DbConnection.IsConnected)
            DbConnection.Start();

        var npgsqlConnection = DbConnection.NpgsqlConnection;
        var state = DbConnection.IsConnected;
        if (!state)
        {
            throw new NpgsqlException("Не удалось подключиться к базе данных");
        }

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
        var npgsqlCommand = new NpgsqlCommand(query, npgsqlConnection);
        return npgsqlCommand.ExecuteReader();
    }

    public static IEnumerable<Book> GetItems()
    {
        var dataReader = GetDataReader();
        
        while (dataReader.Read())
        {
            yield return new Book{
                Id = Convert.ToInt32(dataReader["id"].ToString() ?? string.Empty),
                Name = dataReader["name"].ToString() ?? string.Empty,
                Author = dataReader["author"].ToString() ?? string.Empty,
                Category = dataReader["category"].ToString() ?? string.Empty,
                Style = dataReader["style"].ToString() ?? string.Empty
            };
        }

        if (dataReader is {IsClosed: false})
            dataReader.Close();
    }
}