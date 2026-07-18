using InfoTrack.Solicitors.Core.Models;

namespace InfoTrack.Solicitors.Core.Search;

/// <summary>
/// Runs a conveyancing search across a set of locations, persists the run, and flags
/// solicitors that weren't present in the previous run for the same location.
/// </summary>
public interface ISearchOrchestrator
{
    Task<SearchRunSummary> RunSearchAsync(IEnumerable<string> locations, CancellationToken cancellationToken = default);
}
