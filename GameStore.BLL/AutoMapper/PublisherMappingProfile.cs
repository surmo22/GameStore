using AutoMapper;
using GameStore.BLL.DTOs.Publisher;
using GameStore.Domain.Entities.CoreEntities;

namespace GameStore.BLL.AutoMapper;

public class PublisherMappingProfile : Profile
{
    public PublisherMappingProfile()
    {
        CreateMap<PublisherDto, Publisher>()
            .ForMember(dest => dest.Games, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ReverseMap();
        CreateMap<PublisherCreateDto, Publisher>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Games, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
    }
}