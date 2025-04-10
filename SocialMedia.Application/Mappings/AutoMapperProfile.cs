using AutoMapper;
using Infrastructure;
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
    }
}