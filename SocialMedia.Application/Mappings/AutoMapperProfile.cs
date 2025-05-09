using AutoMapper;
using Domain.Entities;
using SocialMedia.Application.DTOs;

namespace SocialMedia.Application.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Post, CreatePostDto>();
        CreateMap<Post, UpdatePostDto>();
        CreateMap<CreatePostDto, Post>();
        CreateMap<UpdatePostDto, Post>();
        CreateMap<UpdateUserDto, User>();
        CreateMap<User, UserProfileDto>();
        CreateMap<RegisterDto, User>();
        CreateMap<PostLike, PostLikeDto>();
        CreateMap<Follow, FollowDto>();
        CreateMap<User, UsernameDto>();
        CreateMap<Post, PostFeedDto>();
    }
}