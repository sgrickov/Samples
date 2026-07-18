using InfoTrack.Solicitors.Core.Reporting;
using InfoTrack.Solicitors.Data;
using InfoTrack.Solicitors.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfoTrack.Solicitors.Tests.Reporting;

public class ReportBuilderTests
{
    private static AppDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static SolicitorListingEntity Listing(
        string name, decimal rating, int reviewCount, bool accredited = false, bool isNew = false) => new()
    {
        Name = name,
        Address = "1 Test Street",
        Rating = rating,
        ReviewCount = reviewCount,
        ProfileUrl = $"/{name.Replace(' ', '-').ToLowerInvariant()}.html",
        IsNewSinceLastRun = isNew,
        QualityMarks = accredited
            ? [new SolicitorQualityMarkEntity { MarkName = "Lexcel" }]
            : []
    };

    [Fact]
    public async Task BuildReportAsync_ReturnsNull_WhenRunDoesNotExist()
    {
        await using var dbContext = CreateInMemoryDbContext();
        var builder = new ReportBuilder(dbContext);

        var report = await builder.BuildReportAsync(999);

        Assert.Null(report);
    }

    [Fact]
    public async Task BuildReportAsync_ReturnsNull_WhenRunHasNotCompleted()
    {
        await using var dbContext = CreateInMemoryDbContext();
        dbContext.SearchRuns.Add(new SearchRunEntity { StartedAtUtc = DateTime.UtcNow, CompletedAtUtc = null });
        await dbContext.SaveChangesAsync();
        var builder = new ReportBuilder(dbContext);

        var report = await builder.BuildReportAsync(1);

        Assert.Null(report);
    }

    [Fact]
    public async Task BuildReportAsync_ComputesNationalAndPerLocationInsight()
    {
        await using var dbContext = CreateInMemoryDbContext();

        var run = new SearchRunEntity
        {
            StartedAtUtc = DateTime.UtcNow.AddMinutes(-1),
            CompletedAtUtc = DateTime.UtcNow,
            Locations =
            [
                new SearchRunLocationEntity
                {
                    LocationName = "London",
                    ResultCount = 3,
                    Listings =
                    [
                        Listing("Top Firm", 5.0m, 100, accredited: true),
                        Listing("Mid Firm", 4.0m, 50, accredited: true, isNew: true),
                        Listing("Low Firm", 2.0m, 5)
                    ]
                }
            ]
        };

        dbContext.SearchRuns.Add(run);
        await dbContext.SaveChangesAsync();

        var builder = new ReportBuilder(dbContext);
        var report = await builder.BuildReportAsync(run.Id);

        Assert.NotNull(report);
        Assert.Equal(3, report.National.TotalSolicitorsFound);
        Assert.Equal(2, report.National.AccreditedCount);
        Assert.Equal(66.7, report.National.AccreditedPercentage, 1);
        Assert.Equal(new[] { "Top Firm", "Mid Firm", "Low Firm" }, report.National.TopRated.Select(l => l.Name));

        var london = report.Locations.Single();
        Assert.Equal("Mid Firm", london.NewSinceLastRun.Single().Name);
    }

    [Fact]
    public async Task GetLatestSearchRunIdAsync_ReturnsHighestId()
    {
        await using var dbContext = CreateInMemoryDbContext();
        dbContext.SearchRuns.AddRange(
            new SearchRunEntity { StartedAtUtc = DateTime.UtcNow, CompletedAtUtc = DateTime.UtcNow },
            new SearchRunEntity { StartedAtUtc = DateTime.UtcNow, CompletedAtUtc = DateTime.UtcNow });
        await dbContext.SaveChangesAsync();

        var latestId = await new ReportBuilder(dbContext).GetLatestSearchRunIdAsync();

        Assert.Equal(2, latestId);
    }

    [Fact]
    public async Task GetHistoryAsync_ReturnsRunsNewestFirstWithTotals()
    {
        await using var dbContext = CreateInMemoryDbContext();
        dbContext.SearchRuns.AddRange(
            new SearchRunEntity
            {
                StartedAtUtc = DateTime.UtcNow,
                CompletedAtUtc = DateTime.UtcNow,
                Locations = [new SearchRunLocationEntity { LocationName = "London", ResultCount = 5 }]
            },
            new SearchRunEntity
            {
                StartedAtUtc = DateTime.UtcNow,
                CompletedAtUtc = DateTime.UtcNow,
                Locations = [new SearchRunLocationEntity { LocationName = "London", ResultCount = 7 }]
            });
        await dbContext.SaveChangesAsync();

        var history = await new ReportBuilder(dbContext).GetHistoryAsync();

        Assert.Equal(2, history.Count);
        Assert.Equal(7, history[0].TotalFound);
        Assert.Equal(5, history[1].TotalFound);
    }
}
