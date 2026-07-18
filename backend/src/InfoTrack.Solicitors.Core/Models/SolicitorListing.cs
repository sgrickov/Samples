namespace InfoTrack.Solicitors.Core.Models;

public class SolicitorListing
{
    public required string Name { get; init; }
    public required string Address { get; init; }
    public string? Phone { get; init; }
    public decimal Rating { get; init; }
    public int ReviewCount { get; init; }
    public required string ProfileUrl { get; init; }
    public string? WebsiteUrl { get; init; }
    public string? EmailFormUrl { get; init; }
    public string? Description { get; init; }
    public bool IsBasicListing { get; init; }
    public IReadOnlyList<string> QualityMarks { get; init; } = [];
}
