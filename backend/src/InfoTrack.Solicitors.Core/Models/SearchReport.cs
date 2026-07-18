namespace InfoTrack.Solicitors.Core.Models;

/// <summary>The "standard report layout": raw scraped data turned into per-location and national insight.</summary>
public class SearchReport
{
    public int SearchRunId { get; init; }
    public DateTime StartedAtUtc { get; init; }
    public DateTime CompletedAtUtc { get; init; }
    public required NationalSummary National { get; init; }
    public IReadOnlyList<LocationReport> Locations { get; init; } = [];
}

public class NationalSummary
{
    public int TotalLocationsSearched { get; init; }
    public int TotalSolicitorsFound { get; init; }
    public decimal AverageRating { get; init; }
    public int AccreditedCount { get; init; }
    public double AccreditedPercentage { get; init; }
    public IReadOnlyList<SolicitorListingView> TopRated { get; init; } = [];
}

public class LocationReport
{
    public required string LocationName { get; init; }
    public int TotalFound { get; init; }
    public decimal AverageRating { get; init; }
    public int AccreditedCount { get; init; }
    public IReadOnlyList<SolicitorListingView> TopRated { get; init; } = [];
    public IReadOnlyList<SolicitorListingView> NewSinceLastRun { get; init; } = [];
    public IReadOnlyList<SolicitorListingView> AllListings { get; init; } = [];
}

public class SolicitorListingView
{
    public required string Name { get; init; }
    public required string LocationName { get; init; }
    public required string Address { get; init; }
    public string? Phone { get; init; }
    public decimal Rating { get; init; }
    public int ReviewCount { get; init; }
    public bool IsAccredited { get; init; }
    public string? WebsiteUrl { get; init; }
    public bool IsNewSinceLastRun { get; init; }
}
