using InfoTrack.Solicitors.Api.Contracts;
using InfoTrack.Solicitors.Data;
using InfoTrack.Solicitors.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InfoTrack.Solicitors.Api.Controllers;

[ApiController]
[Route("api/locations")]
public class LocationsController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<string>>> GetLocations(CancellationToken cancellationToken)
    {
        var names = await ActiveLocationNamesAsync(cancellationToken);
        return Ok(names);
    }

    /// <summary>Replaces the full saved location list with the given names (add/remove reconciled by name).</summary>
    [HttpPut]
    public async Task<ActionResult<IReadOnlyList<string>>> UpdateLocations(
        UpdateLocationsRequest request, CancellationToken cancellationToken)
    {
        var incoming = request.Locations
            .Select(name => name.Trim())
            .Where(name => name.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (incoming.Count == 0)
        {
            return BadRequest("At least one location is required.");
        }

        var existing = await dbContext.Locations.ToListAsync(cancellationToken);

        var noLongerWanted = existing.Where(e => !incoming.Contains(e.Name, StringComparer.OrdinalIgnoreCase));
        dbContext.Locations.RemoveRange(noLongerWanted);

        var existingNames = existing.Select(e => e.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var newLocations = incoming
            .Where(name => !existingNames.Contains(name))
            .Select(name => new LocationEntity { Name = name, IsActive = true });
        dbContext.Locations.AddRange(newLocations);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Ok(await ActiveLocationNamesAsync(cancellationToken));
    }

    private Task<List<string>> ActiveLocationNamesAsync(CancellationToken cancellationToken) =>
        dbContext.Locations
            .Where(l => l.IsActive)
            .OrderBy(l => l.Name)
            .Select(l => l.Name)
            .ToListAsync(cancellationToken);
}
