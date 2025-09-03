using AutoMapper;
using GameStore.BLL.DTOs;
using GameStore.Domain.MongoEntities;

namespace GameStore.BLL.AutoMapper;

public class ShipperMappingProfile : Profile
{
    public ShipperMappingProfile()
    {
        CreateMap<MongoShipper, GetShipperDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.ShipperId, opt => opt.MapFrom(src => src.ShipperId))
            .ForMember(dest => dest.AdditionalFields, opt => opt.MapFrom(src => src.AdditionalFields))
            .ReverseMap();
    }
}