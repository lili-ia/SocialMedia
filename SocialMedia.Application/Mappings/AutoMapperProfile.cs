

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
        CreateMap<Post, PostDto>();
        
        CreateMap<UpdateUserDto, User>();
        CreateMap<User, PrivateUserProfileDto>();
        CreateMap<User, PublicUserProfileDto>();
        CreateMap<RegisterDto, User>();
        
        CreateMap<PostLike, PostLikeDto>();
        CreateMap<Follow, FollowDto>();
        CreateMap<User, UserPreviewDto>();
        
        CreateMap<Comment, CommentDto>();
    }
}