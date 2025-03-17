using Common.DataContract;

namespace Common.Services;

public interface IJournalService
{
    IAsyncEnumerable<Item> GetJournalEntries(int page = 1);
}