using AutoMapper;
using GameStore.BLL.DTOs.Games;
using GameStore.Domain.Entities.CoreEntities;

namespace GameStore.BLL.AutoMapper;

public class GameMappingProfile : Profile
{
    public GameMappingProfile()
    {
        CreateMap<GameDto, Game>()
            .ForMember(dest => dest.Genres, opt => opt.Ignore())
            .ForMember(dest => dest.Platforms, opt => opt.Ignore())
            .ForMember(dest => dest.Publisher, opt => opt.Ignore())
            .ForMember(dest => dest.PublisherId, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.QuantityPerUnit, opt => opt.Ignore())
            .ForMember(dest => dest.UnitsOnOrder, opt => opt.Ignore())
            .ForMember(dest => dest.ReorderLevel, opt => opt.Ignore())
            .ForMember(dest => dest.Discontinued, opt => opt.Ignore())
            .ForMember(dest => dest.CreationDate, opt => opt.Ignore())
            .ForMember(dest => dest.ViewCount, opt => opt.Ignore())
            .ForMember(dest => dest.Comments, opt => opt.Ignore())
            .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
            .ReverseMap();

        CreateMap<GameCreateDto, Game>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Genres, opt => opt.Ignore())
            .ForMember(dest => dest.Platforms, opt => opt.Ignore())
            .ForMember(dest => dest.Publisher, opt => opt.Ignore())
            .ForMember(dest => dest.PublisherId, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.QuantityPerUnit, opt => opt.Ignore())
            .ForMember(dest => dest.UnitsOnOrder, opt => opt.Ignore())
            .ForMember(dest => dest.ReorderLevel, opt => opt.Ignore())
            .ForMember(dest => dest.Discontinued, opt => opt.Ignore())
            .ForMember(dest => dest.CreationDate, opt => opt.Ignore())
            .ForMember(dest => dest.ViewCount, opt => opt.Ignore())
            .ForMember(dest => dest.Comments, opt => opt.Ignore())
            .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
            .ReverseMap();
    }
}