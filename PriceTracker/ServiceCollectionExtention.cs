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
            services.AddHttpClient<IPullAndBearClient, PullAndBearClient>();
            services.AddHttpClient<IBershkaClient, BershkaClient>();
        }

        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IPullAndBearParser, PullAndBearParser>();
            services.AddSingleton<IBershkaParser, BershkaParser>();
            services.AddSingleton<IUpdateInfoHelper, UpdateInfoHelper>();
            services.AddSingleton<ITextConverter, TextConverter>();
            services.AddSingleton<IShopDefiner, ShopDefiner>();
            services.AddSingleton<IBotService, BotService>();
            services.AddScoped<ITrackingRepository, TrackingRepository>();
            services.AddScoped<IShopService, ShopService>();
            services.AddScoped<ITimerService, TimerService>();
            services.AddScoped<IUpdateService, UpdateService>();
        }
    }
}
