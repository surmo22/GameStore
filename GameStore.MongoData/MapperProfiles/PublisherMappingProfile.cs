using AutoMapper;
using GameStore.Common.Utils;
using GameStore.Domain.Entities.CoreEntities;
using GameStore.Domain.MongoEntities;

namespace GameStore.MongoData.MapperProfiles;

public class PublisherMappingProfile : Profile
{
    public PublisherMappingProfile()
    {
        CreateMap<MongoPublisher, Publisher>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(p => IntToGuidConverter.Convert(p.SupplierId)))
            .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(p => p.CompanyName));
    }
}