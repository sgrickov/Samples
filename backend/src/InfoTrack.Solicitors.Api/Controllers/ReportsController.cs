using InfoTrack.Solicitors.Core.Models;
using InfoTrack.Solicitors.Core.Reporting;
using InfoTrack.Solicitors.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InfoTrack.Solicitors.Api.Controllers;

[ApiController]
[Route("api/reports")]
public class ReportsController(IReportBuilder reportBuilder, AppDbContext dbContext) : ControllerBase
{
    [HttpGet("latest")]
    public async Task<ActionResult<SearchReport>> GetLatest(CancellationToken cancellationToken)
    {
        var latestId = await reportBuilder.GetLatestSearchRunIdAsync(cancellationToken);
        if (latestId is null)
        {
            return NotFound("No search has been run yet.");
        }

        var report = await reportBuilder.BuildReportAsync(latestId.Value, cancellationToken);
        return report is null
            ? NotFound("The latest search run has not completed yet.")
            : Ok(report);
    }

    [HttpGet("history")]
    public async Task<ActionResult<IReadOnlyList<SearchRunHistoryItem>>> GetHistory(CancellationToken cancellationToken)
    {
        var history = await reportBuilder.GetHistoryAsync(cancellationToken);
        return Ok(history);
    }

    [HttpGet("{searchRunId:int}")]
    public async Task<ActionResult<SearchReport>> GetByRunId(int searchRunId, CancellationToken cancellationToken)
    {
        var report = await reportBuilder.BuildReportAsync(searchRunId, cancellationToken);
        return report is null ? NotFound() : Ok(report);
    }

    /// <summary>Deletes one search run (cascades to its locations/listings/quality marks via FK cascade).</summary>
    [HttpDelete("{searchRunId:int}")]
    public async Task<IActionResult> DeleteRun(int searchRunId, CancellationToken cancellationToken)
    {
        var run = await dbContext.SearchRuns.FindAsync([searchRunId], cancellationToken);
        if (run is null)
        {
            return NotFound();
        }

        dbContext.SearchRuns.Remove(run);
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    /// <summary>Deletes every search run — clears the entire history.</summary>
    [HttpDelete]
    public async Task<IActionResult> DeleteAll(CancellationToken cancellationToken)
    {
        await dbContext.SearchRuns.ExecuteDeleteAsync(cancellationToken);
        return NoContent();
    }
}
