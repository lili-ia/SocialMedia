using Domain.Enums;
using Domain.Events;
using Domain.Exceptions;

namespace Domain.Entities;

public sealed class User : BaseEntity
{

    public string UsernameNormalized { get; private set; } = null!;
    public string EmailNormalized { get; private set; } = null!;

    public DateOnly BirthDate { get; private set; }

    public string PasswordHash { get; private set; } = null!;

    private UserStatus _status = UserStatus.Pending;

    public UserStatus Status
    {
        get => _status;
        private set
        {
            _status = value;
            StatusChangedAt = DateTime.UtcNow;
        }
    }

    public UserRole UserRole { get; private set; } = UserRole.User;

    public string? Bio { get; private set; }

    public DateTime? LastSeen { get; private set; }

    public Guid? CurrentProfilePicId { get; private set; }
    
    public ProfilePic CurrentProfilePic { get; }

    public DateTime? StatusChangedAt { get; private set; }

    public string? StatusReason { get; private set; }

    public DateTime? LastEmailSentAt { get; private set; }

    public IReadOnlyCollection<Post> Posts => _posts.AsReadOnly();
    public IReadOnlyCollection<PostLike> PostLikes => _postLikes.AsReadOnly();
    public IReadOnlyCollection<Block> BlockedUsers => _blockedUsers.AsReadOnly();
    
    public IReadOnlyCollection<Block> BlockedByUsers => _blockedUsers.AsReadOnly();
    
    public IReadOnlyCollection<Message> Messages => _messages.AsReadOnly();
    
    public IReadOnlyCollection<Follow> Followees => _followees.AsReadOnly();
    
    public IReadOnlyCollection<Follow> Followers => _followers.AsReadOnly();
    
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();
    
    public IReadOnlyCollection<PostView> PostViews => _postViews.AsReadOnly();
    
    public IReadOnlyCollection<Notification> Notifications => _notifications.AsReadOnly();

    private User() { }

    public static User Create(
        string username,
        string email,
        string passwordHash,
        DateOnly birthDate)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new DomainValidationException("Username is required.");
        }
        
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new DomainValidationException("Email is required.");
        }

        var user = new User
        {
            UsernameNormalized = username.ToLower(),
            EmailNormalized = email.ToLower(),
            PasswordHash = passwordHash,
            BirthDate = birthDate,
            Status = UserStatus.Pending,
            LastEmailSentAt = DateTime.UtcNow
        };

        return user;
    }

    public void UpdateProfile(string? bio, DateOnly? birthDate)
    {
        if (bio != null)
        {
            Bio = bio;
        }

        if (birthDate.HasValue)
        {
            if (birthDate.Value > DateOnly.FromDateTime(DateTime.UtcNow))
            {
                throw new DomainValidationException("Birth date cannot be in future.");
            }

            BirthDate = birthDate.Value;
        }

        MarkAsUpdated();
        AddDomainEvent(new ProfileUpdatedEvent(Id));
    }
    
    public void UpdateLastSeen()
    {
        LastSeen = DateTime.UtcNow;
    }

    public void UpdatePassword(string passwordHash)
    {
        PasswordHash = passwordHash;
    }

    public void ChangeStatus(UserStatus status, string? reason = null)
    {
        Status = status;
        StatusReason = reason;
        
        AddDomainEvent(new ProfileUpdatedEvent(Id));
    }

    public void UpdateProfilePicture(Guid pictureId)
    {
        CurrentProfilePicId = pictureId;
        MarkAsUpdated();

        AddDomainEvent(new ProfileUpdatedEvent(Id));
    }
    
    public void RecordEmailSent()
    {
        if (CanSendEmail() == false)
        {
            throw new RateLimitDomainException("Email sending cooldown not respected.");
        }

        LastEmailSentAt = DateTime.UtcNow;
    }

    public bool CanSendEmail(int cooldownMinutes = 2)
    {
        if (!LastEmailSentAt.HasValue)
        {
            return true;
        }

        return DateTime.UtcNow > LastEmailSentAt.Value.AddMinutes(cooldownMinutes);
    }
    
    private readonly List<Post> _posts = [];
    private readonly List<Follow> _followers = [];
    private readonly List<Follow> _followees = [];
    private readonly List<Block> _blockedUsers = [];
    private readonly List<Block> _blockedByUsers = [];
    private readonly List<PostLike> _postLikes = [];
    private readonly List<Message> _messages = [];
    private readonly List<RefreshToken> _refreshTokens = [];
    private readonly List<PostView> _postViews = [];
    private readonly List<Notification> _notifications = [];
}