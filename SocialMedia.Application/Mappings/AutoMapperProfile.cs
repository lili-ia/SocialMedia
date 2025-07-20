using AutoMapper;
using Domain.Entities;
using SocialMedia.Application.DTOs.Auth;
using SocialMedia.Application.DTOs.Comment;
using SocialMedia.Application.DTOs.Follow;
using SocialMedia.Application.DTOs.Like;
using SocialMedia.Application.DTOs.Post;
using SocialMedia.Application.DTOs.User;

namespace SocialMedia.Application.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Post, CreatePostRequest>();

        CreateMap<Post, UpdatePostDto>();
        
        CreateMap<CreatePostRequest, Post>();
        
        CreateMap<UpdatePostDto, Post>();

        CreateMap<Post, PostDto>()
            .ForMember(dest => dest.PostId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.CommentsCount, opt => opt.MapFrom(src => src.Comments.Count))
            .ForMember(dest => dest.LikesCount, opt => opt.MapFrom(src => src.PostLikes.Count));
        
        CreateMap<UpdateUserDto, User>();
        
        CreateMap<User, PublicUserProfileDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.FollowersCount, opt => opt.MapFrom(src => src.Followers.Count))
            .ForMember(dest => dest.FolloweesCount, opt => opt.MapFrom(src => src.Followees.Count))
            .ForMember(dest => dest.PostsCount, opt => opt.MapFrom(src => src.Posts.Count));
        
        CreateMap<User, PrivateUserProfileDto>();
        
        CreateMap<RegisterDto, User>();
        
        CreateMap<PostLike, PostLikeDto>();
        
        CreateMap<Follow, FollowDto>();
        
        CreateMap<User, UserPreviewDto>();
        
        CreateMap<Comment, CommentDto>();
    }
}