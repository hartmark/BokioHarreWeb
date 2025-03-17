namespace Common.DataContract;

public record GetJournalEntriesResponse
{
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public List<Item>? Items { get; set; }
}