using System.Net;
using System.Text.RegularExpressions;

namespace InfoTrack.Solicitors.Core.Parsing;

/// <summary>
/// Single-purpose helpers for pulling one data point at a time out of an already-isolated
/// HTML fragment (as produced by <see cref="HtmlBlockExtractor"/>).
/// </summary>
public static partial class HtmlFieldExtractors
{
    /// <summary>
    /// Returns the text that sits directly inside an element, before any nested child tag —
    /// e.g. the solicitor name that precedes the quality-mark/rating markup inside a "h2" span.
    /// </summary>
    public static string? ExtractDirectText(string elementOuterHtml)
    {
        var openTagEnd = elementOuterHtml.IndexOf('>') + 1;
        var closeTagStart = elementOuterHtml.LastIndexOf('<');
        if (openTagEnd <= 0 || closeTagStart <= openTagEnd)
        {
            return null;
        }

        var inner = elementOuterHtml[openTagEnd..closeTagStart];
        var firstChildTagIndex = inner.IndexOf('<');
        var directText = firstChildTagIndex >= 0 ? inner[..firstChildTagIndex] : inner;

        return DecodeAndClean(directText);
    }

    /// <summary>
    /// Returns the full text content of an element with all nested tags stripped —
    /// e.g. a description paragraph or an address block.
    /// </summary>
    public static string? ExtractAllText(string elementOuterHtml)
    {
        var openTagEnd = elementOuterHtml.IndexOf('>') + 1;
        var closeTagStart = elementOuterHtml.LastIndexOf('<');
        if (openTagEnd <= 0 || closeTagStart <= openTagEnd)
        {
            return null;
        }

        var inner = elementOuterHtml[openTagEnd..closeTagStart];
        var withoutTags = StripTagsRegex().Replace(inner, " ");

        return DecodeAndClean(withoutTags);
    }

    /// <summary>
    /// Finds the first tel: link in the fragment and returns its display text (the human-readable
    /// phone number), covering both the "phone-block" (large listing) and bare "a.tel" (small listing) markup.
    /// </summary>
    public static string? ExtractTelPhoneNumber(string fragment)
    {
        var match = TelLinkRegex().Match(fragment);
        return match.Success ? DecodeAndClean(match.Groups["text"].Value) : null;
    }

    /// <summary>
    /// Parses a 5-star rating widget (five sibling "star-full"/"star-half"/"star-none" elements
    /// followed by a "(N)" review count) into a 0-5 decimal rating and a review count.
    /// </summary>
    public static (decimal Rating, int ReviewCount) ExtractStarRating(string fragment)
    {
        var ratingSpan = HtmlBlockExtractor.ExtractElementsByClass(fragment, "span", "rev-results").FirstOrDefault()
            ?? fragment;

        var fullStars = StarTokenRegex("star-full").Matches(ratingSpan).Count;
        var halfStars = StarTokenRegex("star-half").Matches(ratingSpan).Count;
        var rating = fullStars * 1.0m + halfStars * 0.5m;

        var reviewCountMatch = ReviewCountRegex().Match(ratingSpan);
        var reviewCount = reviewCountMatch.Success ? int.Parse(reviewCountMatch.Groups["count"].Value) : 0;

        return (rating, reviewCount);
    }

    /// <summary>
    /// Reads the quality-mark titles off a "greentick"/"greentick-small" badge, if present.
    /// The badge's title attribute is a free-text sentence followed by one mark per line.
    /// </summary>
    public static IReadOnlyList<string> ExtractQualityMarks(string fragment)
    {
        var badge = HtmlBlockExtractor.ExtractElementsByClass(fragment, "div", "greentick").FirstOrDefault()
            ?? HtmlBlockExtractor.ExtractElementsByClass(fragment, "div", "greentick-small").FirstOrDefault();

        if (badge is null)
        {
            return [];
        }

        var title = HtmlBlockExtractor.ExtractAttribute(badge, "title");
        if (string.IsNullOrWhiteSpace(title))
        {
            return [];
        }

        return title
            .Replace("\r\n", "\n")
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Skip(1) // first line is the "X hold the following quality marks:" lead-in
            .Select(line => WebUtility.HtmlDecode(line).Trim().TrimEnd('.').Trim())
            .Where(mark => mark.Length > 0)
            .ToList();
    }

    private static string DecodeAndClean(string raw) =>
        WhitespaceRegex().Replace(WebUtility.HtmlDecode(raw), " ").Trim();

    [GeneratedRegex(@"<a\b[^>]*href\s*=\s*""tel:[^""]*""[^>]*>(?<text>.*?)</a>", RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex TelLinkRegex();

    [GeneratedRegex(@"<[^>]+>")]
    private static partial Regex StripTagsRegex();

    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespaceRegex();

    [GeneratedRegex(@"\((?<count>\d+)\)")]
    private static partial Regex ReviewCountRegex();

    private static Regex StarTokenRegex(string starClassName) => new(
        $@"\bclass\s*=\s*""[^""]*\b{Regex.Escape(starClassName)}\b[^""]*""", RegexOptions.IgnoreCase);
}
