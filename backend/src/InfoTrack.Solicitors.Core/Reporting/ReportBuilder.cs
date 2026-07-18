using InfoTrack.Solicitors.Core.Models;
using InfoTrack.Solicitors.Data;
using InfoTrack.Solicitors.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfoTrack.Solicitors.Core.Reporting;

public class ReportBuilder(AppDbContext dbContext) : IReportBuilder
{
    private const int TopRatedCount = 5;

    public Task<int?> GetLatestSearchRunIdAsync(CancellationToken cancellationToken = default) =>
        dbContext.SearchRuns
            .OrderByDescending(r => r.Id)
            .Select(r => (int?)r.Id)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyList<SearchRunHistoryItem>> GetHistoryAsync(CancellationToken cancellationToken = default) =>
        await dbContext.SearchRuns
            .OrderByDescending(r => r.Id)
            .Select(r => new SearchRunHistoryItem
            {
                SearchRunId = r.Id,
                StartedAtUtc = r.StartedAtUtc,
                CompletedAtUtc = r.CompletedAtUtc,
                LocationsSearched = r.Locations.Count,
                TotalFound = r.Locations.Sum(l => l.ResultCount)
            })
            .ToListAsync(cancellationToken);

    public async Task<SearchReport?> BuildReportAsync(int searchRunId, CancellationToken cancellationToken = default)
    {
        var run = await dbContext.SearchRuns
            .Include(r => r.Locations)
                .ThenInclude(l => l.Listings)
                    .ThenInclude(s => s.QualityMarks)
            .FirstOrDefaultAsync(r => r.Id == searchRunId, cancellationToken);

        if (run?.CompletedAtUtc is null)
        {
            return null;
        }

        var locationReports = run.Locations.Select(BuildLocationReport).ToList();
        var allListings = locationReports.SelectMany(l => l.AllListings).ToList();

        return new SearchReport
        {
            SearchRunId = run.Id,
            StartedAtUtc = run.StartedAtUtc,
            CompletedAtUtc = run.CompletedAtUtc.Value,
            National = BuildNationalSummary(locationReports.Count, allListings),
            Locations = locationReports
        };
    }

    private static NationalSummary BuildNationalSummary(int locationsSearched, IReadOnlyList<SolicitorListingView> allListings) => new()
    {
        TotalLocationsSearched = locationsSearched,
        TotalSolicitorsFound = allListings.Count,
        AverageRating = AverageRatingOf(allListings),
        AccreditedCount = allListings.Count(v => v.IsAccredited),
        AccreditedPercentage = allListings.Count > 0
            ? Math.Round(allListings.Count(v => v.IsAccredited) * 100.0 / allListings.Count, 1)
            : 0,
        TopRated = RankTopRated(allListings)
    };

    private static LocationReport BuildLocationReport(SearchRunLocationEntity location)
    {
        var listingViews = location.Listings.Select(entity => ToView(entity, location.LocationName)).ToList();

        return new LocationReport
        {
            LocationName = location.LocationName,
            TotalFound = listingViews.Count,
            AverageRating = AverageRatingOf(listingViews),
            AccreditedCount = listingViews.Count(v => v.IsAccredited),
            TopRated = RankTopRated(listingViews),
            NewSinceLastRun = listingViews.Where(v => v.IsNewSinceLastRun).ToList(),
            AllListings = listingViews
        };
    }

    private static decimal AverageRatingOf(IReadOnlyList<SolicitorListingView> listings) =>
        listings.Count > 0 ? Math.Round(listings.Average(v => v.Rating), 2) : 0m;

    private static IReadOnlyList<SolicitorListingView> RankTopRated(IEnumerable<SolicitorListingView> listings) =>
        listings
            .OrderByDescending(l => l.Rating)
            .ThenByDescending(l => l.ReviewCount)
            .Take(TopRatedCount)
            .ToList();

    private static SolicitorListingView ToView(SolicitorListingEntity entity, string locationName) => new()
    {
        Name = entity.Name,
        LocationName = locationName,
        Address = entity.Address,
        Phone = entity.Phone,
        Rating = entity.Rating,
        ReviewCount = entity.ReviewCount,
        IsAccredited = entity.QualityMarks.Count > 0,
        WebsiteUrl = entity.WebsiteUrl,
        IsNewSinceLastRun = entity.IsNewSinceLastRun
    };
}
