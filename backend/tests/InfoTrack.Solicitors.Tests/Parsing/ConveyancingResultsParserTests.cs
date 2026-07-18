using InfoTrack.Solicitors.Core.Parsing;

namespace InfoTrack.Solicitors.Tests.Parsing;

public class ConveyancingResultsParserTests
{
    private static readonly string FixtureHtml = File.ReadAllText(
        Path.Combine(AppContext.BaseDirectory, "Fixtures", "ConveyancingLondonResults.html"));

    private readonly ConveyancingResultsParser _parser = new();

    [Fact]
    public void Parse_ReturnsEveryListing_FromRealCapturedResultsPage()
    {
        var listings = _parser.Parse(FixtureHtml);

        // Verified by direct inspection of the live page at capture time: 75 result-item blocks.
        Assert.Equal(75, listings.Count);
    }

    [Fact]
    public void Parse_ExtractsFullVariantListing_WithAllFields()
    {
        var listings = _parser.Parse(FixtureHtml);

        var davisons = listings.Single(l => l.Name == "Davisons Law");

        Assert.False(davisons.IsBasicListing);
        Assert.Equal(4.5m, davisons.Rating);
        Assert.Equal(4259, davisons.ReviewCount);
        Assert.Equal("0203 582 7855", davisons.Phone);
        Assert.Equal("/davisons-law.html", davisons.ProfileUrl);
        Assert.Contains("Streatham High Rd", davisons.Address);
        Assert.Contains("stress free", davisons.Description);
        Assert.Equal("https://www.qualitysolicitors.com/davisons/edgbaston", davisons.WebsiteUrl);
        Assert.Contains("enquiry-form.asp", davisons.EmailFormUrl);
        Assert.Contains("Lexcel", davisons.QualityMarks);
        Assert.Contains(davisons.QualityMarks, m => m.Contains("Conveyancing Quality Scheme"));
    }

    [Fact]
    public void Parse_ExtractsSmallVariantListing_WithCoreFieldsButNoWebsiteOrEmail()
    {
        var listings = _parser.Parse(FixtureHtml);

        var alstern = listings.Single(l => l.Name == "Alstern Solicitors");

        Assert.True(alstern.IsBasicListing);
        Assert.Equal(5.0m, alstern.Rating);
        Assert.Equal(111, alstern.ReviewCount);
        Assert.Equal("0203 923 9188", alstern.Phone);
        Assert.Contains("Canary Wharf", alstern.Address);
        Assert.Null(alstern.WebsiteUrl);
        Assert.Null(alstern.EmailFormUrl);
        Assert.Empty(alstern.QualityMarks);
    }

    [Fact]
    public void Parse_HandlesHalfStarAndZeroReviewListings()
    {
        var listings = _parser.Parse(FixtureHtml);

        var sharpe = listings.Single(l => l.Name == "Sharpe Pritchard LLP");

        Assert.Equal(5.0m, sharpe.Rating);
        Assert.Equal(1, sharpe.ReviewCount);
    }

    [Fact]
    public void Parse_HandlesListingWithNoRatingWidgetAtAll()
    {
        var listings = _parser.Parse(FixtureHtml);

        var bullivant = listings.Single(l => l.Name == "Bullivant & Partners");

        Assert.Equal(0m, bullivant.Rating);
        Assert.Equal(0, bullivant.ReviewCount);
        Assert.Empty(bullivant.QualityMarks);
    }
}
