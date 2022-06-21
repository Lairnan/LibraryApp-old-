namespace DbConnect.Models;

public class Reader
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string? Patronymic { get; set; }
    public DateTime Birthday { get; set; }
    public Type Type { get; set; }
}