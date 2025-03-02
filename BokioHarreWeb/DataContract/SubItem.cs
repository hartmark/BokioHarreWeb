namespace BokioHarreWeb.DataContract;

public record SubItem
{
    public int Id { get; set; }
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public int Account { get; set; }
}