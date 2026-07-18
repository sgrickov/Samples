namespace InfoTrack.Solicitors.Core.Models;

/// <summary>Returned immediately after a search run completes.</summary>
public class SearchRunSummary
{
    public int SearchRunId { get; init; }
    public DateTime StartedAtUtc { get; init; }
    public DateTime CompletedAtUtc { get; init; }
    public IReadOnlyList<LocationSearchSummary> Locations { get; init; } = [];
}

public class LocationSearchSummary
{
    public required string LocationName { get; init; }
    public int ResultCount { get; init; }
    public int NewSinceLastRunCount { get; init; }
}
