using InfoTrack.Solicitors.Core.Models;
using InfoTrack.Solicitors.Core.Options;
using InfoTrack.Solicitors.Core.Parsing;
using InfoTrack.Solicitors.Core.Scraping;
using InfoTrack.Solicitors.Data;
using InfoTrack.Solicitors.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace InfoTrack.Solicitors.Core.Search;

public class SearchOrchestrator(
    ISolicitorsComClient client,
    IConveyancingResultsParser parser,
    AppDbContext dbContext,
    IOptions<ScraperOptions> options) : ISearchOrchestrator
{
    private readonly ScraperOptions _options = options.Value;

    public async Task<SearchRunSummary> RunSearchAsync(IEnumerable<string> locations, CancellationToken cancellationToken = default)
    {
        var locationNames = locations.ToList();
        if (locationNames.Count == 0)
        {
            throw new ArgumentException("At least one location is required.", nameof(locations));
        }

        var run = new SearchRunEntity { StartedAtUtc = DateTime.UtcNow };
        dbContext.SearchRuns.Add(run);
        await dbContext.SaveChangesAsync(cancellationToken);

        var locationSummaries = new List<LocationSearchSummary>();

        for (var i = 0; i < locationNames.Count; i++)
        {
            if (i > 0)
            {
                await Task.Delay(_options.DelayBetweenRequestsMs, cancellationToken);
            }

            locationSummaries.Add(await SearchOneLocationAsync(run.Id, locationNames[i], cancellationToken));
        }

        run.CompletedAtUtc = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        return new SearchRunSummary
        {
            SearchRunId = run.Id,
            StartedAtUtc = run.StartedAtUtc,
            CompletedAtUtc = run.CompletedAtUtc.Value,
            Locations = locationSummaries
        };
    }

    private async Task<LocationSearchSummary> SearchOneLocationAsync(int searchRunId, string locationName, CancellationToken cancellationToken)
    {
        var html = await client.SubmitConveyancingSearchAsync(locationName, cancellationToken);
        var listings = parser.Parse(html);

        var (hasPreviousRun, previousProfileUrls) = await GetPreviousRunProfileUrlsAsync(locationName, cancellationToken);

        var runLocation = new SearchRunLocationEntity
        {
            SearchRunId = searchRunId,
            LocationName = locationName,
            ResultCount = listings.Count
        };

        foreach (var listing in listings)
        {
            // Nothing to compare the very first time a location is searched — don't flag
            // an entire fresh result set as "new", only genuine appearances on later runs.
            var isNew = hasPreviousRun && !previousProfileUrls.Contains(listing.ProfileUrl);
            runLocation.Listings.Add(ToEntity(listing, isNew));
        }

        dbContext.SearchRunLocations.Add(runLocation);

        return new LocationSearchSummary
        {
            LocationName = locationName,
            ResultCount = listings.Count,
            NewSinceLastRunCount = runLocation.Listings.Count(l => l.IsNewSinceLastRun)
        };
    }

    /// <summary>
    /// The set of profile-page URLs seen in the most recent prior run for this location, used as
    /// the natural key to detect newly-appeared firms. Returns an empty set the first time a
    /// location is searched (nothing to compare against yet).
    /// </summary>
    private async Task<(bool HasPreviousRun, HashSet<string> ProfileUrls)> GetPreviousRunProfileUrlsAsync(
        string locationName, CancellationToken cancellationToken)
    {
        var previousLocationRun = await dbContext.SearchRunLocations
            .Where(l => l.LocationName == locationName)
            .OrderByDescending(l => l.Id)
            .Include(l => l.Listings)
            .FirstOrDefaultAsync(cancellationToken);

        return previousLocationRun is null
            ? (false, [])
            : (true, previousLocationRun.Listings.Select(l => l.ProfileUrl).ToHashSet());
    }

    private static SolicitorListingEntity ToEntity(SolicitorListing listing, bool isNew) => new()
    {
        Name = listing.Name,
        Address = listing.Address,
        Phone = listing.Phone,
        Rating = listing.Rating,
        ReviewCount = listing.ReviewCount,
        ProfileUrl = listing.ProfileUrl,
        WebsiteUrl = listing.WebsiteUrl,
        EmailFormUrl = listing.EmailFormUrl,
        Description = listing.Description,
        IsBasicListing = listing.IsBasicListing,
        IsNewSinceLastRun = isNew,
        QualityMarks = listing.QualityMarks
            .Select(mark => new SolicitorQualityMarkEntity { MarkName = mark })
            .ToList()
    };
}
