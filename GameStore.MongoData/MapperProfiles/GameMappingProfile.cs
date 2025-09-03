using AutoMapper;
using GameStore.Common.Utils;
using GameStore.Domain.Entities.CoreEntities;
using GameStore.Domain.MongoEntities;

namespace GameStore.MongoData.MapperProfiles;

public class GameMappingProfile : Profile
{
    public GameMappingProfile()
    {
        CreateMap<MongoGame, Game>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(p => IntToGuidConverter.Convert(p.ProductId)))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ProductName))
            .ForMember(dest => dest.Key, opt => opt.MapFrom(src => src.ProductKey))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.UnitPrice))
            .ForMember(dest => dest.UnitInStock, opt => opt.MapFrom(src => src.UnitsInStock))
            .ForMember(dest => dest.Publisher, opt => opt.Ignore())
            .ForMember(dest => dest.Genres, opt => opt.MapFrom(p => new List<Genre>()))
            .ForMember(dest => dest.Platforms, opt => opt.MapFrom(p => new List<Platform>()));
    }
}