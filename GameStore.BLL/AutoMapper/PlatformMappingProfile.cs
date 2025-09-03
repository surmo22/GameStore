using AutoMapper;
using GameStore.BLL.DTOs.Platforms;
using GameStore.Domain.Entities.CoreEntities;

namespace GameStore.BLL.AutoMapper;

public class PlatformMappingProfile : Profile
{
    public PlatformMappingProfile()
    {
        CreateMap<PlatformDto, Platform>()
            .ForMember(dest => dest.Games, opt => opt.Ignore())
            .ReverseMap();
        CreateMap<PlatformCreateDto, Platform>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Games, opt => opt.Ignore());
    }
}
