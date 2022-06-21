using DbConnect.Items;

Console.WriteLine(string.Join("\n\r",Books.Get().Select(book=>$"{book.Id}, {book.Name}, {book.Author}")));

try
{
    Console.WriteLine(Books.Remove(3));
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

Console.WriteLine(string.Join("\n\r",Authors.Get().Select(author=>$"{author.Id}, {author.Name}, {author.Surname}")));

GC.Collect();
