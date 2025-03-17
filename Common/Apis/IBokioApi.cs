using Common.DataContract;
using Refit;

namespace Common.Apis;

public interface IBokioApi
{
    [Get("/companies/{companyId}/journal-entries")]
    Task<IApiResponse<GetJournalEntriesResponse>> GetJournalEntries(
        int page, int pageSize = 100,
        string companyId = "companyId-secret");
}