using System.Text.RegularExpressions;

namespace InfoTrack.Solicitors.Core.Parsing;

/// <summary>
/// Generic, markup-agnostic helper for pulling balanced element fragments out of raw HTML
/// by tag name (optionally filtered by CSS class), without any 3rd-party HTML parsing library.
/// </summary>
public static class HtmlBlockExtractor
{
    /// <summary>
    /// Finds every &lt;tagName&gt; element in <paramref name="html"/> and returns each one's
    /// full outer HTML, correctly accounting for nested elements of the same tag name.
    /// </summary>
    public static IEnumerable<string> ExtractElementsByTag(string html, string tagName)
    {
        var openTagPattern = OpenTagRegex(tagName);

        foreach (Match match in openTagPattern.Matches(html))
        {
            var fragmentStart = match.Index;
            var searchFrom = match.Index + match.Length;
            var fragmentEnd = FindMatchingCloseTagEnd(html, tagName, searchFrom);
            if (fragmentEnd is null)
            {
                continue;
            }

            yield return html[fragmentStart..fragmentEnd.Value];
        }
    }

    /// <summary>
    /// As <see cref="ExtractElementsByTag"/>, further filtered to elements whose class attribute
    /// contains <paramref name="className"/> as a whitespace-delimited token (not a substring match).
    /// </summary>
    public static IEnumerable<string> ExtractElementsByClass(string html, string tagName, string className)
    {
        foreach (var element in ExtractElementsByTag(html, tagName))
        {
            var openingTag = element[..(element.IndexOf('>') + 1)];
            var classMatch = ClassAttributeRegex.Match(openingTag);

            if (classMatch.Success && HasClassToken(classMatch.Groups["class"].Value, className))
            {
                yield return element;
            }
        }
    }

    /// <summary>
    /// Reads an attribute's value from an element's opening tag (as returned by the Extract* methods above).
    /// </summary>
    public static string? ExtractAttribute(string elementOuterHtml, string attributeName)
    {
        var openingTag = elementOuterHtml[..(elementOuterHtml.IndexOf('>') + 1)];
        var match = new Regex($@"\b{Regex.Escape(attributeName)}\s*=\s*""(?<value>[^""]*)""", RegexOptions.IgnoreCase)
            .Match(openingTag);

        return match.Success ? match.Groups["value"].Value : null;
    }

    private static bool HasClassToken(string classAttributeValue, string className)
    {
        foreach (var token in classAttributeValue.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            if (token.Equals(className, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Starting just after an opening tag (depth 1), walks forward counting nested open/close
    /// tags of the same name until depth returns to 0, and returns the index just past the
    /// matching closing tag. Returns null if the markup never closes (malformed/truncated HTML).
    /// </summary>
    private static int? FindMatchingCloseTagEnd(string html, string tagName, int searchFrom)
    {
        var tagPattern = OpenOrCloseTagRegex(tagName);
        var depth = 1;

        foreach (Match match in tagPattern.Matches(html, searchFrom))
        {
            var isClosingTag = match.Groups["slash"].Success;
            depth += isClosingTag ? -1 : 1;

            if (depth == 0)
            {
                return match.Index + match.Length;
            }
        }

        return null;
    }

    private static readonly Regex ClassAttributeRegex =
        new(@"\bclass\s*=\s*""(?<class>[^""]*)""", RegexOptions.IgnoreCase);

    private static Regex OpenTagRegex(string tagName) => new(
        $@"<{Regex.Escape(tagName)}\b[^>]*>",
        RegexOptions.IgnoreCase);

    private static Regex OpenOrCloseTagRegex(string tagName) => new(
        $@"<(?<slash>/)?{Regex.Escape(tagName)}\b[^>]*>",
        RegexOptions.IgnoreCase);
}
