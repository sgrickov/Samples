namespace InfoTrack.Solicitors.Core.Options;

public class ScraperOptions
{
    public const string SectionName = "Scraper";

    public string BaseUrl { get; set; } = "https://www.solicitors.com";
    public int ConveyancingCategoryId { get; set; } = 192;
    public string UserAgent { get; set; } =
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36";
    public int DelayBetweenRequestsMs { get; set; } = 500;
}
