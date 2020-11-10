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
            services.AddHttpClient<IShopClient, PullAndBearClient>();
        }

        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IParser, PullAndBearParser>();
            services.AddSingleton<IUpdateInfoHelper, UpdateInfoHelper>();
            services.AddSingleton<IBotService, BotService>();
            services.AddScoped<ITrackingRepository, TrackingRepository>();
            services.AddScoped<IShopService, ShopService>();
            services.AddScoped<IUpdateService, UpdateService>();
            services.AddSingleton<IShopDefiner, ShopDefiner>();
        }
    }
}
