using AutoMapper;
using GameStore.Common.Utils;
using GameStore.Domain.Entities.CoreEntities;
using GameStore.Domain.Entities.Relations;
using GameStore.Domain.MongoEntities;

namespace GameStore.MongoData.MapperProfiles;

public class OrderProfileMapper : Profile
{
    public OrderProfileMapper()
    {
        CreateMap<MongoOrder, Order>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => IntToGuidConverter.Convert(src.OrderId)))
            .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => IntToGuidConverter.Convert(src.CustomerId)))
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.OrderDate))
            .ForMember(dest => dest.Items, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore());

        CreateMap<MongoOrderDetails, OrderGame>()
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => IntToGuidConverter.Convert(src.OrderId)))
            .ForMember(dest => dest.Order, opt => opt.Ignore())
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => IntToGuidConverter.Convert(src.ProductId)))
            .ForMember(dest => dest.Product, opt => opt.Ignore())
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.UnitPrice))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));

    }
}