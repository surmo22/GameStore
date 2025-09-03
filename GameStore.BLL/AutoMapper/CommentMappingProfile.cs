using AutoMapper;
using GameStore.BLL.DTOs.Comment;
using GameStore.BLL.Utils;
using GameStore.Domain.Entities.CoreEntities;

namespace GameStore.BLL.AutoMapper;

public class CommentMappingProfile : Profile
{
    public CommentMappingProfile()
    {
        CreateMap<Comment, GetCommentDto>()
            .ForMember(dest => dest.ChildComments,
                opt => opt.MapFrom(src => src.ChildComments))
            .ForMember(dest => dest.Name,
                opt => opt.MapFrom(src => src.Name));

        CreateMap<AddCommentRequestDto, Comment>()
            .ForMember(dest => dest.Body,
                opt => opt.MapFrom(src => src.AddComment.Body))
            .ForMember(dest => dest.ParentId,
                opt => opt.MapFrom(src => GuidConverter.ConvertStringToGuid(src.ParentId)))
            .ForMember(dest => dest.Id,
                opt => opt.Ignore())
            .ForMember(dest => dest.GameId,
                opt => opt.Ignore())
            .ForMember(dest => dest.Game,
                opt => opt.Ignore())
            .ForMember(dest => dest.Parent,
                opt => opt.Ignore())
            .ForMember(dest => dest.ChildComments,
                opt => opt.Ignore())
            .ForMember(dest => dest.UserId,
                opt => opt.Ignore())
            .ForMember(dest => dest.User,
                opt => opt.Ignore());
    }
}