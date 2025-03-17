using Microsoft.Extensions.Configuration;

namespace Common.Handlers;

public class TokenHandler : DelegatingHandler
{
    private readonly string _token;
    private readonly string _companyId;

    public TokenHandler(IConfiguration configuration)
    {
        var token = configuration["BokioToken"];
        if (string.IsNullOrEmpty(token) || token.Contains("TODO"))
        {
            throw new ApplicationException("Bokio token is missing");
        }
            
        _token = token;

        var companyId = configuration["CompanyId"];
        if (string.IsNullOrEmpty(companyId) || companyId.Contains("TODO"))
        {
            throw new ApplicationException("CompanyId is missing");
        }
            
        _companyId = companyId;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

        const string companyIdPlaceholder = "companyId-secret";
        if (!request.RequestUri!.ToString().Contains(companyIdPlaceholder))
        {
            return await base.SendAsync(request, cancellationToken);
        }
            
        var uriBuilder = new UriBuilder(request.RequestUri);
        var path = uriBuilder.Path.Replace(companyIdPlaceholder, _companyId);
        uriBuilder.Path = path;
        request.RequestUri = uriBuilder.Uri;

        return await base.SendAsync(request, cancellationToken);
    }
}