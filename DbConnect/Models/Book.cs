namespace DbConnect.Models;

public class Book
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Author { get; set; }
    public string? Category { get; set; }
    public string? Style { get; set; }
}