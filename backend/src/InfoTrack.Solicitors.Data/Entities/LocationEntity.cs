namespace InfoTrack.Solicitors.Data.Entities;

public class LocationEntity
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public bool IsActive { get; set; } = true;
}
