using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PriceTracker.Clients;
using PriceTracker.Helpers;
using PriceTracker.Repositories;
using PriceTracker.Services;

namespace PriceTracker
{
    public static class ServiceCollectionExtention
    {
        public static void AddHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<IASOSClient, ASOSClient>();
        }

        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IParserHelper, ParserHelper>();
            services.AddSingleton<IBotService, BotService>();
            services.AddScoped<ITrackingRepository, TrackingRepository>();
            services.AddScoped<IASOSService, ASOSService>();
            services.AddScoped<IUpdateService, UpdateService>();
        }
    }
}
