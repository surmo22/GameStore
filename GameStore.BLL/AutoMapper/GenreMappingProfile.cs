using AutoMapper;
using GameStore.BLL.DTOs.Genres;
using GameStore.BLL.Utils;
using GameStore.Domain.Entities.CoreEntities;

namespace GameStore.BLL.AutoMapper;

public class GenreMappingProfile : Profile
{
    public GenreMappingProfile()
    {
        CreateMap<GenreDto, Genre>()
            .ForMember(dest => dest.ParentGenreId, opt =>
                opt.MapFrom(src => GuidConverter.ConvertStringToGuid(src.ParentGenreId)))
            .ForMember(dest => dest.ParentGenre, opt => opt.Ignore())
            .ForMember(dest => dest.SubGenres, opt => opt.Ignore())
            .ForMember(dest => dest.Games, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ReverseMap()
            .ForMember(dest => dest.ParentGenreId, opt =>
                opt.MapFrom(src => src.ParentGenreId.ToString()));

        CreateMap<GenreCreateDto, Genre>()
            .ForMember(dest => dest.ParentGenreId, opt
                => opt.MapFrom(src => GuidConverter.ConvertStringToGuid(src.ParentGenreId)))
            .ForMember(dest => dest.ParentGenre, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.SubGenres, opt => opt.Ignore())
            .ForMember(dest => dest.Games, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
    }
}