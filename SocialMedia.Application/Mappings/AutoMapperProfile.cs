using AutoMapper;
using Infrastructure.Models;
using SocialMedia.Application.DTOs;
using SocialMedia.DTOs;

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
    }
}