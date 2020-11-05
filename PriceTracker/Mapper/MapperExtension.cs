using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace PriceTracker.Mapper
{
    public static class MapperExtension
    {
        public static void AddMapper(this IServiceCollection services)
        {
            services.AddSingleton(provider => {
                var profiles = new List<Profile> { new ItemMapperProfile() };
                var mapperConfiguration = new MapperConfiguration(mc => mc.AddProfiles(profiles));
                var mapper = mapperConfiguration.CreateMapper();
                return mapper;
            });        
        }
    }
}
