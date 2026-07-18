namespace InfoTrack.Solicitors.Data.Entities;

public class SolicitorQualityMarkEntity
{
    public int Id { get; set; }
    public int SolicitorListingId { get; set; }
    public required string MarkName { get; set; }

    public SolicitorListingEntity? SolicitorListing { get; set; }
}
