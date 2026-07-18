namespace InfoTrack.Solicitors.Data.Entities;

public class SearchRunLocationEntity
{
    public int Id { get; set; }
    public int SearchRunId { get; set; }
    public required string LocationName { get; set; }
    public int ResultCount { get; set; }

    public SearchRunEntity? SearchRun { get; set; }
    public List<SolicitorListingEntity> Listings { get; set; } = [];
}
