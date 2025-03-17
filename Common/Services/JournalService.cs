using Common.Apis;
using Common.DataContract;

namespace Common.Services;

public class JournalService(IBokioApi bokioApi) : IJournalService
{
    public async IAsyncEnumerable<Item> GetJournalEntries(int page = 1)
    {
        var response = await bokioApi.GetJournalEntries(page);

        if (!response.IsSuccessful)
        {
            throw response.Error;
        }
        
        if (response.Content.Items == null)
        {
            yield break;
        }
        
        var items = response.Content.Items;

        foreach (var item in items)
        {
            yield return item;
        }

        if (response.Content.CurrentPage == response.Content.TotalPages)
        {
            yield break;
        }
        
        var nextPage = GetJournalEntries(++page);
        await foreach (var items2 in nextPage)
        {
            yield return items2;
        }
    }
}