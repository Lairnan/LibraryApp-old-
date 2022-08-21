using DbConnect.Models;
using Npgsql;

namespace DbConnect.Items;

public static class Books
{
    public static int Add(string name, int author, int category, int style)
    {
        var npgsqlConnection = DbConnection.NpgsqlConnection;
        var state = DbConnection.IsConnected;
        if (!state)
        {
            throw new NpgsqlException("Не удалось подключиться к базе данных");
        }

        var selCmd = new NpgsqlCommand("SELECT * FROM books " +
                                       "WHERE lower(name) = @name " +
                                       "AND author_id = @author_id",
            npgsqlConnection);
        selCmd.Parameters.AddWithValue("name", name.ToLower());
        selCmd.Parameters.AddWithValue("author_id", author);

        var dataReader = selCmd.ExecuteScalar();
        if (dataReader != null) throw new Exception("Данная запись уже существует в базе данных");
        
        var insCmd =
            new NpgsqlCommand("INSERT INTO books (name, author_id, category_id, style_id) VALUES (@name,@author_id,@categorie_id,@style_id)",
                npgsqlConnection);
        insCmd.Parameters.AddWithValue("name", name);
        insCmd.Parameters.AddWithValue("author_id", author);
        insCmd.Parameters.AddWithValue("categorie_id", category);
        insCmd.Parameters.AddWithValue("style_id", style);
        
        var result = insCmd.ExecuteNonQuery();

        return result;
    }

    private static NpgsqlDataReader GetDataReader()
    {
        var npgsqlConnection = DbConnection.NpgsqlConnection;
        var state = DbConnection.IsConnected;
        if (!state)
        {
            throw new NpgsqlException("Не удалось подключиться к базе данных");
        }

        const string query = "SELECT b.id as id" +
                             ", b.name as name" +
                             ", a.id as \"authorId\"" +
                             ", a.surname as \"authorSurname\"" +
                             ", a.name as \"authorName\"" +
                             ", a.patronymic as \"authorPatronymic\"" +
                             ", c.id as categoryId" +
                             ", c.name as categoryName" +
                             ", s.id as styleId " +
                             ", s.name as styleName " +
                             "FROM books b " +
                             "LEFT JOIN authors a on b.author_id = a.id " +
                             "LEFT JOIN categories c on b.category_id = c.id " +
                             "LEFT JOIN styles s on b.style_id = s.id " +
                             "ORDER BY b.id";
        var npgsqlCommand = new NpgsqlCommand(query, npgsqlConnection);
        return npgsqlCommand.ExecuteReader();
    }

    public static IEnumerable<Book> Get()
    {
        var dataReader = GetDataReader();
        
        while (dataReader.Read())
        {
            yield return new Book{
                Id = Convert.ToInt32(dataReader["id"].ToString() ?? string.Empty),
                Name = dataReader["name"].ToString() ?? string.Empty,
                Author = new Author
                {
                    Id = Convert.ToInt32(dataReader["authorId"].ToString() ?? string.Empty) - 1,
                    Name = dataReader["authorName"].ToString() ?? string.Empty,
                    Surname = dataReader["authorSurname"].ToString() ?? string.Empty,
                    Patronymic = dataReader["authorPatronymic"].ToString() != "" ? dataReader["authorPatronymic"].ToString() : null
                },
                Category = new Category
                {
                    Id = Convert.ToInt32(dataReader["categoryId"].ToString() ?? string.Empty) - 1,
                    Name = dataReader["categoryName"].ToString() ?? string.Empty
                },
                Style = new Style
                {
                    Id = Convert.ToInt32(dataReader["styleId"].ToString() ?? string.Empty) - 1,
                    Name = dataReader["styleName"].ToString() ?? string.Empty
                },
            };
        }

        if (dataReader is {IsClosed: false})
            dataReader.Close();
        
    }

    public static int Update(int id, string name, int author, int category, int style)
    {
        if (name == string.Empty) throw new Exception("Поля не должны быть пустыми");
        
        var npgsqlConnection = DbConnection.NpgsqlConnection;
        var state = DbConnection.IsConnected;
        if (!state)
        {
            throw new NpgsqlException("Не удалось подключиться к базе данных");
        }
        
        var selCmd = new NpgsqlCommand("SELECT * FROM books " +
                                       "WHERE id = @id",
            npgsqlConnection);
        selCmd.Parameters.AddWithValue("id", id);

        var data = selCmd.ExecuteScalar();
        if (data == null) throw new Exception("Выбранной записи не существует");
        
        
        var insCmd =
            new NpgsqlCommand("UPDATE books SET name = @name, author_id = @author, category_id = @category, style_id = @style WHERE id = @id",
                npgsqlConnection);
        insCmd.Parameters.AddWithValue("id", id);
        insCmd.Parameters.AddWithValue("name", name);
        insCmd.Parameters.AddWithValue("author", author);
        insCmd.Parameters.AddWithValue("category", category);
        insCmd.Parameters.AddWithValue("style", style);

        var result = insCmd.ExecuteNonQuery();

        return result;
    }

    public static int Remove(int id)
    {
        var npgsqlConnection = DbConnection.NpgsqlConnection;
        var state = DbConnection.IsConnected;
        if (!state)
        {
            throw new NpgsqlException("Не удалось подключиться к базе данных");
        }
        
        var selCmd = new NpgsqlCommand("SELECT * FROM books " +
                                       "WHERE id = @id",
            npgsqlConnection);
        selCmd.Parameters.AddWithValue("id", id);

        var data = selCmd.ExecuteScalar();
        if (data == null) throw new Exception("Выбранной записи не существует");

        var remCmd = new NpgsqlCommand("DELETE FROM books WHERE id = @id", npgsqlConnection);
        remCmd.Parameters.AddWithValue("id",id);

        return remCmd.ExecuteNonQuery();
    }
}