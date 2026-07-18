namespace InfoTrack.Solicitors.Data.Entities;

public class SearchRunEntity
{
    public int Id { get; set; }
    public DateTime StartedAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }

    public List<SearchRunLocationEntity> Locations { get; set; } = [];
}
