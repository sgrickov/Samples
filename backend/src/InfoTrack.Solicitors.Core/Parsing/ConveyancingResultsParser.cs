using InfoTrack.Solicitors.Core.Models;

namespace InfoTrack.Solicitors.Core.Parsing;

/// <summary>
/// The one class in the codebase that knows about solicitors.com-specific markup. It composes
/// the generic <see cref="HtmlBlockExtractor"/> with the focused <see cref="HtmlFieldExtractors"/>
/// to map each "result-item" fragment (both the full and the "item-small" listing variants) to
/// a <see cref="SolicitorListing"/>.
/// </summary>
public class ConveyancingResultsParser : IConveyancingResultsParser
{
    public IReadOnlyList<SolicitorListing> Parse(string html)
    {
        var listings = new List<SolicitorListing>();

        foreach (var fragment in HtmlBlockExtractor.ExtractElementsByClass(html, "div", "result-item"))
        {
            var listing = ParseListing(fragment);
            if (listing is not null)
            {
                listings.Add(listing);
            }
        }

        return listings;
    }

    private static SolicitorListing? ParseListing(string fragment)
    {
        var nameElement = HtmlBlockExtractor.ExtractElementsByClass(fragment, "span", "h2").FirstOrDefault();
        var name = nameElement is not null ? HtmlFieldExtractors.ExtractDirectText(nameElement) : null;
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        var (address, profileUrl) = ExtractAddressAndProfileUrl(fragment);
        var (rating, reviewCount) = HtmlFieldExtractors.ExtractStarRating(fragment);
        var (description, emailFormUrl, websiteUrl) = ExtractDescriptionAndLinks(fragment);

        return new SolicitorListing
        {
            Name = name,
            Address = address ?? string.Empty,
            Phone = HtmlFieldExtractors.ExtractTelPhoneNumber(fragment),
            Rating = rating,
            ReviewCount = reviewCount,
            ProfileUrl = profileUrl ?? string.Empty,
            WebsiteUrl = websiteUrl,
            EmailFormUrl = emailFormUrl,
            Description = description,
            IsBasicListing = IsBasicVariant(fragment),
            QualityMarks = HtmlFieldExtractors.ExtractQualityMarks(fragment)
        };
    }

    private static (string? Address, string? ProfileUrl) ExtractAddressAndProfileUrl(string fragment)
    {
        var addressLink = HtmlBlockExtractor.ExtractElementsByClass(fragment, "a", "link-map").FirstOrDefault();
        if (addressLink is null)
        {
            return (null, null);
        }

        var profileUrl = HtmlBlockExtractor.ExtractAttribute(addressLink, "href");
        var addressElement = HtmlBlockExtractor.ExtractElementsByTag(addressLink, "address").FirstOrDefault();
        var address = addressElement is not null ? HtmlFieldExtractors.ExtractAllText(addressElement) : null;

        return (address, profileUrl);
    }

    /// <summary>
    /// The description paragraph and email/website links are present on most, but not all,
    /// listings regardless of variant — extraction is presence-driven rather than gated by
    /// the large/small distinction.
    /// </summary>
    private static (string? Description, string? EmailFormUrl, string? WebsiteUrl) ExtractDescriptionAndLinks(string fragment)
    {
        var description = HtmlBlockExtractor.ExtractElementsByTag(fragment, "p")
            .Select(HtmlFieldExtractors.ExtractAllText)
            .FirstOrDefault(text => !string.IsNullOrWhiteSpace(text));

        string? emailFormUrl = null;
        string? websiteUrl = null;

        foreach (var link in HtmlBlockExtractor.ExtractElementsByTag(fragment, "a"))
        {
            var href = HtmlBlockExtractor.ExtractAttribute(link, "href");
            if (string.IsNullOrEmpty(href))
            {
                continue;
            }

            if (href.Contains("enquiry-form", StringComparison.OrdinalIgnoreCase))
            {
                emailFormUrl = href;
            }
            else if (HtmlFieldExtractors.ExtractAllText(link)?.Contains("Website", StringComparison.OrdinalIgnoreCase) == true)
            {
                websiteUrl = href;
            }
        }

        return (description, emailFormUrl, websiteUrl);
    }

    private static bool IsBasicVariant(string fragment)
    {
        var openingTag = fragment[..(fragment.IndexOf('>') + 1)];
        return HtmlBlockExtractor.ExtractAttribute(openingTag, "class")?
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Contains("item-small", StringComparer.OrdinalIgnoreCase) == true;
    }
}
