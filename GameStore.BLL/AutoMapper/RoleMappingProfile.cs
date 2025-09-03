using AutoMapper;
using GameStore.BLL.DTOs.Role;
using GameStore.Domain.Entities.UserEntities;

namespace GameStore.BLL.AutoMapper;

public class RoleMappingProfile : Profile
{
    public RoleMappingProfile()
    {
        CreateMap<Role, RoleDto>()
            .ForMember(dest => dest.Id, option => option.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, option => option.MapFrom(src => src.Name));
    }
}