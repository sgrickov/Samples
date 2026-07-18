using InfoTrack.Solicitors.Core.Options;
using Microsoft.Extensions.Options;

namespace InfoTrack.Solicitors.Core.Scraping;

/// <summary>
/// Performs the same form POST a browser would make from /conveyancing.html: submits
/// did=&lt;ConveyancingCategoryId&gt;&amp;location=&lt;location&gt; to /prepare-search.asp and
/// follows the resulting redirect to the results page (HttpClient follows redirects by default).
/// </summary>
public class SolicitorsComClient(HttpClient httpClient, IOptions<ScraperOptions> options) : ISolicitorsComClient
{
    private readonly ScraperOptions _options = options.Value;

    public async Task<string> SubmitConveyancingSearchAsync(string location, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/prepare-search.asp")
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["did"] = _options.ConveyancingCategoryId.ToString(),
                ["location"] = location
            })
        };

        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync(cancellationToken);
    }
}
