using InfoTrack.Solicitors.Core.Options;
using InfoTrack.Solicitors.Core.Parsing;
using InfoTrack.Solicitors.Core.Search;
using InfoTrack.Solicitors.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace InfoTrack.Solicitors.Tests.Search;

public class SearchOrchestratorTests
{
    private static AppDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private static SearchOrchestrator CreateOrchestrator(AppDbContext dbContext, FakeSolicitorsComClient client) =>
        new(client, new ConveyancingResultsParser(), dbContext, Options.Create(new ScraperOptions { DelayBetweenRequestsMs = 0 }));

    [Fact]
    public async Task RunSearchAsync_PersistsListingsAndReturnsAccurateSummary()
    {
        await using var dbContext = CreateInMemoryDbContext();
        var html = FakeSolicitorsComClient.BuildResultsHtml(("Firm A", "firm-a"), ("Firm B", "firm-b"));
        var orchestrator = CreateOrchestrator(dbContext, new FakeSolicitorsComClient(html));

        var summary = await orchestrator.RunSearchAsync(["London"]);

        Assert.Equal(2, summary.Locations.Single().ResultCount);
        Assert.Equal(2, await dbContext.SolicitorListings.CountAsync());
    }

    [Fact]
    public async Task RunSearchAsync_FirstEverRunForALocation_DoesNotFlagAnyoneAsNew()
    {
        await using var dbContext = CreateInMemoryDbContext();
        var html = FakeSolicitorsComClient.BuildResultsHtml(("Firm A", "firm-a"), ("Firm B", "firm-b"));
        var orchestrator = CreateOrchestrator(dbContext, new FakeSolicitorsComClient(html));

        var summary = await orchestrator.RunSearchAsync(["London"]);

        Assert.Equal(0, summary.Locations.Single().NewSinceLastRunCount);
        Assert.All(await dbContext.SolicitorListings.ToListAsync(), l => Assert.False(l.IsNewSinceLastRun));
    }

    [Fact]
    public async Task RunSearchAsync_SecondRun_FlagsOnlyGenuinelyNewFirms()
    {
        await using var dbContext = CreateInMemoryDbContext();
        var firstRunHtml = FakeSolicitorsComClient.BuildResultsHtml(("Firm A", "firm-a"), ("Firm B", "firm-b"));
        var secondRunHtml = FakeSolicitorsComClient.BuildResultsHtml(
            ("Firm A", "firm-a"), ("Firm B", "firm-b"), ("Firm C", "firm-c"));

        var firstOrchestrator = CreateOrchestrator(dbContext, new FakeSolicitorsComClient(firstRunHtml));
        await firstOrchestrator.RunSearchAsync(["London"]);

        var secondOrchestrator = CreateOrchestrator(dbContext, new FakeSolicitorsComClient(secondRunHtml));
        var secondSummary = await secondOrchestrator.RunSearchAsync(["London"]);

        Assert.Equal(1, secondSummary.Locations.Single().NewSinceLastRunCount);

        var secondRunListings = await dbContext.SolicitorListings
            .Where(l => l.SearchRunLocation!.SearchRunId == secondSummary.SearchRunId)
            .ToListAsync();

        Assert.True(secondRunListings.Single(l => l.Name == "Firm C").IsNewSinceLastRun);
        Assert.False(secondRunListings.Single(l => l.Name == "Firm A").IsNewSinceLastRun);
        Assert.False(secondRunListings.Single(l => l.Name == "Firm B").IsNewSinceLastRun);
    }

    [Fact]
    public async Task RunSearchAsync_ThrowsForEmptyLocationList()
    {
        await using var dbContext = CreateInMemoryDbContext();
        var orchestrator = CreateOrchestrator(dbContext, new FakeSolicitorsComClient());

        await Assert.ThrowsAsync<ArgumentException>(() => orchestrator.RunSearchAsync([]));
    }
}
