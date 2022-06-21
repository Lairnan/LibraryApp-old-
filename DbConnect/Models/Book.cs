namespace DbConnect.Models;

public class Book
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Author? Author { get; set; }
    public Category? Category { get; set; }
    public Style? Style { get; set; }
}