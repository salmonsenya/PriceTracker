using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PriceTracker.Clients;
using PriceTracker.Repositories;
using PriceTracker.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            services.AddSingleton<IBotService, BotService>();
            services.AddScoped<IUpdateService, UpdateService>();
            services.AddSingleton<IASOSService, ASOSService>();
            services.AddSingleton<ITrackingRepository, TrackingRepository>();
        }
    }
}
