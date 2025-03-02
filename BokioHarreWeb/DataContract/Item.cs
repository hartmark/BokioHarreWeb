namespace BokioHarreWeb.DataContract;

public record Item
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public DateTime Date { get; set; }
    public string? JournalEntryNumber { get; set; }
    public List<SubItem>? Items { get; set; }
    public Guid? ReversingJournalEntryId { get; set; }
    public Guid? ReversedByJournalEntryId { get; set; }
}