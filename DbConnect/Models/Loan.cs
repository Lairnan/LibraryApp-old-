namespace DbConnect.Models;

public class Loan
{
    public int Id { get; set; }
    public string Book { get; set; }
    public string Reader { get; set; }
    public DateTime TakenDate { get; set; }
    public bool Passed { get; set; }
}