using DbConnect;
using DbConnect.Items;

DbConnection.Start();

Console.WriteLine(string.Join("\n\r",Books.Get().Select(book=>$"{book.Name}, {book.Author}")));

DbConnection.Stop();
while(DbConnection.IsConnected)
{
    DbConnection.Stop();
}