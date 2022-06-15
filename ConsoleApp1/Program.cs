using DbConnect;
using DbConnect.Items;

DbConnection.Start();

Console.WriteLine(string.Join("\n\r",Books.Get().Select(book=>$"{book.Id}, {book.Name}, {book.Author}")));

try
{
    Console.WriteLine(Books.Remove(3));
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

Console.WriteLine(string.Join("\n\r",Books.Get().Select(book=>$"{book.Id}, {book.Name}, {book.Author}")));

DbConnection.Stop();
while(DbConnection.IsConnected)
{
    DbConnection.Stop();
}

GC.Collect();
