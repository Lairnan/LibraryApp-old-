namespace DbConnect.Models;

public class Loan
{
    public int Id { get; set; }
    public Book Book { get; set; }
    public Reader Reader { get; set; }
    public DateTime TakenDate { get; set; }
    public bool Passed { get; set; }
}