namespace InfoTrack.Solicitors.Core.Scraping;

/// <summary>
/// Submits the real conveyancing search form on solicitors.com and returns the resulting
/// results-page HTML, exactly as a browser would receive it.
/// </summary>
public interface ISolicitorsComClient
{
    Task<string> SubmitConveyancingSearchAsync(string location, CancellationToken cancellationToken = default);
}
