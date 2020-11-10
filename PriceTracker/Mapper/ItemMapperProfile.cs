using AutoMapper;
using PriceTracker.Models;
using System;
using System.Globalization;

namespace PriceTracker.Mapper
{
    public class ItemMapperProfile : Profile
    {
        public ItemMapperProfile()
        {
            CreateMap<ItemOnline, Item>()
                .ForMember(_ => _.Status, opt => opt.MapFrom(_ => _.Status))
                .ForMember(_ => _.Price, opt => opt.MapFrom(_ => _.Price))
                .ForMember(_ => _.PriceCurrency, opt => opt.MapFrom(_ => _.PriceCurrency))
                .ForMember(_ => _.Name, opt => opt.MapFrom(_ => _.Name))
                .ForMember(_ => _.Image, opt => opt.MapFrom(_ => _.Image));

            CreateMap<PullAndBearProduct, ItemOnline>()
                .ForMember(_ => _.Name, opt => opt.MapFrom(_ => _.name))
                .ForMember(_ => _.Image, opt => opt.MapFrom(_ => _.image))
                .ForMember(_ => _.Price, opt => opt.MapFrom(_ => int.Parse(_.offers.price)))
                .ForMember(_ => _.PriceCurrency, opt => opt.MapFrom(_ => _.offers.priceCurrency));

            CreateMap<BershkaProduct, ItemOnline>()
                .ForMember(_ => _.Name, opt => opt.MapFrom(_ => _.name))
                .ForMember(_ => _.Image, opt => opt.MapFrom(_ => _.image))
                .ForMember(_ => _.Price, opt => opt.MapFrom(_ => Convert.ToInt32(double.Parse(_.offers[0].price, CultureInfo.InvariantCulture))))
                .ForMember(_ => _.PriceCurrency, opt => opt.MapFrom(_ => _.offers[0].priceCurrency));
        }
    }
}
