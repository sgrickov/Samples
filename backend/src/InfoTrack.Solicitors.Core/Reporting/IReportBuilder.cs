using InfoTrack.Solicitors.Core.Models;

namespace InfoTrack.Solicitors.Core.Reporting;

/// <summary>
/// Read-side query surface over persisted search runs — turns raw scraped rows into the
/// "standard report layout" of national + per-location insight.
/// </summary>
public interface IReportBuilder
{
    Task<int?> GetLatestSearchRunIdAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SearchRunHistoryItem>> GetHistoryAsync(CancellationToken cancellationToken = default);
    Task<SearchReport?> BuildReportAsync(int searchRunId, CancellationToken cancellationToken = default);
}
