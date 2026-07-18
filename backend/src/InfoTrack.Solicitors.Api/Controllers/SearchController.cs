using InfoTrack.Solicitors.Api.Contracts;
using InfoTrack.Solicitors.Core.Models;
using InfoTrack.Solicitors.Core.Search;
using InfoTrack.Solicitors.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InfoTrack.Solicitors.Api.Controllers;

[ApiController]
[Route("api/search")]
public class SearchController(ISearchOrchestrator orchestrator, AppDbContext dbContext) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<SearchRunSummary>> RunSearch(
        RunSearchRequest? request, CancellationToken cancellationToken)
    {
        var locations = request?.Locations?
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .ToList();

        if (locations is null || locations.Count == 0)
        {
            locations = await dbContext.Locations
                .Where(l => l.IsActive)
                .OrderBy(l => l.Name)
                .Select(l => l.Name)
                .ToListAsync(cancellationToken);
        }

        if (locations.Count == 0)
        {
            return BadRequest("No locations configured to search.");
        }

        var summary = await orchestrator.RunSearchAsync(locations, cancellationToken);
        return Ok(summary);
    }
}
