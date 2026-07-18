namespace InfoTrack.Solicitors.Core.Models;

/// <summary>One row in the "past runs" history list.</summary>
public class SearchRunHistoryItem
{
    public int SearchRunId { get; init; }
    public DateTime StartedAtUtc { get; init; }
    public DateTime? CompletedAtUtc { get; init; }
    public int LocationsSearched { get; init; }
    public int TotalFound { get; init; }
}
