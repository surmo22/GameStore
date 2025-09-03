using AutoMapper;
using GameStore.BLL.DTOs.User;
using GameStore.Domain.Entities.UserEntities;

namespace GameStore.BLL.AutoMapper;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Id, option => option.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, option => option.MapFrom(src => src.UserName));
    }
}