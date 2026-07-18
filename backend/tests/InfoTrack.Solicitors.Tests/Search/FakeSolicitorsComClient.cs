using InfoTrack.Solicitors.Core.Scraping;

namespace InfoTrack.Solicitors.Tests.Search;

/// <summary>Test double standing in for the real HTTP call to solicitors.com.</summary>
public class FakeSolicitorsComClient : ISolicitorsComClient
{
    private readonly Queue<string> _responsesInCallOrder;

    public FakeSolicitorsComClient(params string[] responsesInCallOrder)
    {
        _responsesInCallOrder = new Queue<string>(responsesInCallOrder);
    }

    public Task<string> SubmitConveyancingSearchAsync(string location, CancellationToken cancellationToken = default) =>
        Task.FromResult(_responsesInCallOrder.Dequeue());

    public static string BuildResultsHtml(params (string Name, string ProfileSlug)[] firms)
    {
        var items = firms.Select(firm => $"""
            <div class="result-item">
                <span class="h2">{firm.Name}<span class="rev-results"><div class="star-full rating-lrg"></div><div class="star-full rating-lrg"></div><div class="star-full rating-lrg"></div><div class="star-full rating-lrg"></div><div class="star-full rating-lrg"></div> (10)</span></span>
                <a href="/{firm.ProfileSlug}.html" class="link-map"><address>1 Test Street, Testville, TE1 1ST</address></a>
            </div>
            """);

        return $"<html><body>{string.Join("\n", items)}</body></html>";
    }
}
