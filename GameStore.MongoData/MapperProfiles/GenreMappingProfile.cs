using AutoMapper;
using GameStore.Common.Utils;
using GameStore.Domain.Entities.CoreEntities;
using GameStore.Domain.MongoEntities;

namespace GameStore.MongoData.MapperProfiles;

public class GenreMappingProfile : Profile
{
    public GenreMappingProfile()
    {
        CreateMap<MongoGenre, Genre>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(g => IntToGuidConverter.Convert(g.CategoryId)))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(g => g.CategoryName));
    }
}