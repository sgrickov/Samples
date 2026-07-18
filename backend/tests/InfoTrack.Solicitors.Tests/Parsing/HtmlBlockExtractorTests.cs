using InfoTrack.Solicitors.Core.Parsing;

namespace InfoTrack.Solicitors.Tests.Parsing;

public class HtmlBlockExtractorTests
{
    [Fact]
    public void ExtractElementsByClass_ReturnsBalancedFragment_WhenElementContainsNestedDivOfSameTag()
    {
        const string html = """
            <div class="result-item">
                <div class="top-holder">nested content</div>
                <p>tail text</p>
            </div>
            <div class="other">should not match</div>
            """;

        var results = HtmlBlockExtractor.ExtractElementsByClass(html, "div", "result-item").ToList();

        Assert.Single(results);
        Assert.Contains("nested content", results[0]);
        Assert.Contains("tail text", results[0]);
        Assert.DoesNotContain("should not match", results[0]);
    }

    [Fact]
    public void ExtractElementsByClass_DoesNotMatchClassNameAsSubstring()
    {
        const string html = """
            <div class="result-item-alt">alt listing, should not match</div>
            <div class="result-item">real listing</div>
            """;

        var results = HtmlBlockExtractor.ExtractElementsByClass(html, "div", "result-item").ToList();

        Assert.Single(results);
        Assert.Contains("real listing", results[0]);
    }

    [Fact]
    public void ExtractElementsByClass_MatchesEitherOrderOfMultipleClassTokens()
    {
        const string html = """<div class="item-small result-item">small listing</div>""";

        var results = HtmlBlockExtractor.ExtractElementsByClass(html, "div", "result-item").ToList();

        Assert.Single(results);
    }

    [Fact]
    public void ExtractAttribute_ReadsValueFromOpeningTag()
    {
        const string element = """<a href="/davisons-law.html" class="link-map">text</a>""";

        var href = HtmlBlockExtractor.ExtractAttribute(element, "href");

        Assert.Equal("/davisons-law.html", href);
    }

    [Fact]
    public void ExtractElementsByTag_HandlesMultipleTopLevelSiblings()
    {
        const string html = "<p>first</p><p>second</p>";

        var results = HtmlBlockExtractor.ExtractElementsByTag(html, "p").ToList();

        Assert.Equal(2, results.Count);
        Assert.Contains("first", results[0]);
        Assert.Contains("second", results[1]);
    }
}
