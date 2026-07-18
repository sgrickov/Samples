using System.Net.Http.Headers;
using InfoTrack.Solicitors.Core.Options;
using InfoTrack.Solicitors.Core.Parsing;
using InfoTrack.Solicitors.Core.Reporting;
using InfoTrack.Solicitors.Core.Scraping;
using InfoTrack.Solicitors.Core.Search;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace InfoTrack.Solicitors.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfoTrackCore(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ScraperOptions>(configuration.GetSection(ScraperOptions.SectionName));

        services.AddHttpClient<ISolicitorsComClient, SolicitorsComClient>((provider, client) =>
        {
            var scraperOptions = provider.GetRequiredService<IOptions<ScraperOptions>>().Value;
            client.BaseAddress = new Uri(scraperOptions.BaseUrl);
            client.DefaultRequestHeaders.UserAgent.Clear();
            client.DefaultRequestHeaders.UserAgent.ParseAdd(scraperOptions.UserAgent);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
        });

        services.AddSingleton<IConveyancingResultsParser, ConveyancingResultsParser>();

        // Scoped: both depend on the per-request/per-scope AppDbContext.
        services.AddScoped<ISearchOrchestrator, SearchOrchestrator>();
        services.AddScoped<IReportBuilder, ReportBuilder>();

        return services;
    }
}
