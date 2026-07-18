namespace InfoTrack.Solicitors.Data.Entities;

public class SolicitorListingEntity
{
    public int Id { get; set; }
    public int SearchRunLocationId { get; set; }

    public required string Name { get; set; }
    public required string Address { get; set; }
    public string? Phone { get; set; }
    public decimal Rating { get; set; }
    public int ReviewCount { get; set; }
    public required string ProfileUrl { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? EmailFormUrl { get; set; }
    public string? Description { get; set; }
    public bool IsBasicListing { get; set; }
    public bool IsNewSinceLastRun { get; set; }

    public SearchRunLocationEntity? SearchRunLocation { get; set; }
    public List<SolicitorQualityMarkEntity> QualityMarks { get; set; } = [];
}
