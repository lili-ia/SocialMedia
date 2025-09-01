using MediatR;
using Microsoft.Extensions.Logging;
using SocialMedia.Application.Common.ResultPattern;
using SocialMedia.Application.Contracts.Repositories;
using SocialMedia.Application.DTOs.User;
using SocialMedia.Application.Mappers;

namespace SocialMedia.Application.Users.GetPrivateInfo;

public class GetPrivateUserInfoCommandHandler : IRequestHandler<GetPrivateUserInfoCommand, Result<UserPrivateDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetPrivateUserInfoCommandHandler> _logger;

    public GetPrivateUserInfoCommandHandler(
        IUserRepository userRepository, 
        ILogger<GetPrivateUserInfoCommandHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result<UserPrivateDto>> Handle(GetPrivateUserInfoCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetPrivateUserInfoCommand {@Command}.", request);

        var user = await _userRepository.GetActiveDetailsByIdAsync(request.UserId, UserMapper.ToUserPrivateDto, cancellationToken);

        if (user is null)
        {
            _logger.LogWarning("User {UserId} not found.", request.UserId);
            
            return Result<UserPrivateDto>.Failure("User not found.", ErrorType.NotFound);
        }

        _logger.LogInformation("Successfully retrieved user {UserId} private profile details.", request.UserId);
        
        return Result<UserPrivateDto>.Success(user);
    }
}