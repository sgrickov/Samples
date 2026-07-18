using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InfoTrack.Solicitors.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfoTrackData(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
        return services;
    }
}
