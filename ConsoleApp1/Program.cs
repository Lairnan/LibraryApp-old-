using DbConnect;
using DbConnect.Items;
using Npgsql;

DbConnection.Start();
var state = DbConnection.IsConnected;

if (!state)
{
    throw new NpgsqlException("Подключение не удалось");
}

foreach (var author in Authors.GetItems())
{
    Console.WriteLine($"{author.Name}, {author.Surname}");
}



/*var data = new Data();
foreach (var dat in data)
{
    Task.Delay(750).Wait();
    Console.WriteLine($"{dat.Id}, {dat.Name}");
}*/

// Console.WriteLine(string.Join("\r\n",new Data()));

// Console.WriteLine("Books: ");
// var books = new GetBooks();
// foreach (var book in books)
// {
//     Console.WriteLine($"{book.Id}: {book.Name}, {book.Author}, {book.Category}, {book.Style}");
// }
//
// Console.WriteLine("\nReaders: ");
// var readers = new GetReaders();
// foreach (var reader in readers)
// {
//     var birthday = reader.Birthday != default ? reader.Birthday.ToString("dd.MM.yy") : string.Empty;
//     Console.WriteLine($"{reader.Id}: {reader.Name}, {reader.Surname}, {reader.Patronymic}, {birthday}, {reader.Type}");
// }
//
// Console.WriteLine("\nAuthors: ");
// var authors = new GetAuthors();
// foreach (var author in authors)
// {
//     Console.WriteLine($"{author.Id}: {author.Name}, {author.Surname}, {author.Patronymic}");
// }
//
// Console.WriteLine("\nCategories: ");
// var categories = new GetCategories();
// foreach (var category in categories)
// {
//     Console.WriteLine($"{category.Id}: {category.Name}");
// }
//
// Console.WriteLine("\nStyles: ");
// var styles = new GetStyles();
// foreach (var style in styles)
// {
//     Console.WriteLine($"{style.Id}: {style.Name}");
// }
//
// Console.WriteLine("\nTypes: ");
// var types = new GetTypes();
// foreach (var type in types)
// {
//     Console.WriteLine($"{type.Id}: {type.Name}");
// }
//
// Console.WriteLine("\nLoans: ");
// var loans = new GetLoans();
// foreach (var loan in loans)
// {
//     var takenDate = loan.TakenDate != default ? loan.TakenDate.ToString("dd.MM.yy") : string.Empty;
//     Console.WriteLine($"{loan.Id}: {loan.Book}, {loan.Reader}, {takenDate}, {loan.Passed}");
// }


// Console.WriteLine("Hello, World!");
//
// var date = DateTime.Now;
// try
// {
//     Console.Write("Введите дату в формате дд.мм.гг: ");
//     var dateTime = DateTime.Parse(Console.ReadLine()!);
//     Console.WriteLine(dateTime.ToString("dd MMM yyyy г. ddd."));
// }
// catch (Exception ex)
// {
//     Console.WriteLine(ex.Message);
// }
//
// Console.WriteLine(date.ToString("dd MMM yyyy г. ddd."));

DbConnection.Stop();
// Console.ReadKey();