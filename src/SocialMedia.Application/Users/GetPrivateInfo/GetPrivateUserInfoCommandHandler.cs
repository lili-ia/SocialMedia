using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.User;
using SocialMedia.Application.Mappers;

namespace SocialMedia.Application.Users.GetPrivateInfo;

public class GetPrivateUserInfoCommandHandler(
    IUserRepository userRepository,
    ILogger<GetPrivateUserInfoCommandHandler> logger)
    : IRequestHandler<GetPrivateUserInfoCommand, Result<UserPrivateDto>>
{
    public async Task<Result<UserPrivateDto>> Handle(GetPrivateUserInfoCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetActiveDetailsByIdAsync(request.UserId, UserMapper.ToUserPrivateDto, cancellationToken);

        if (user is null)
        {
            logger.LogWarning("User {UserId} not found.", request.UserId);
            
            return Result<UserPrivateDto>.Failure("User not found.", ErrorType.NotFound);
        }

        logger.LogInformation("Successfully retrieved user {UserId} private profile details.", request.UserId);
        
        return Result<UserPrivateDto>.Success(user);
    }
}