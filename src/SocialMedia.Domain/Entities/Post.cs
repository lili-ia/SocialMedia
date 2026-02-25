using Domain.Events;
using Domain.Exceptions;
using Microsoft.VisualBasic.CompilerServices;

namespace Domain.Entities;

public sealed class Post : BaseEntity
{
    public string? Text { get; private set; }

    public bool IsHidden { get; private set; }

    public Guid UserId { get; private set; }

    public User User { get; private set; } = null!;

    public int LikeCount => _likes.Count;
    
    public int CommentCount => _comments.Count;
    
    public int ViewCount => _views.Count;

    public IReadOnlyCollection<Comment> Comments => _comments.AsReadOnly();
    
    public IReadOnlyCollection<PostLike> PostLikes => _likes.AsReadOnly();
    
    public IReadOnlyCollection<PostFile> PostFiles => _files.AsReadOnly();
    
    public IReadOnlyCollection<PostView> PostViews => _views.AsReadOnly();

    private Post() { }

    private Post(Guid userId, string? text)
    {
        UserId = userId;
        Text = text;
        IsHidden = false;
    }

    public static Post Create(Guid userId, string? text)
    {
        if (userId == Guid.Empty)
        {
            throw new DomainValidationException("User is required.");
        }
        
        if (text?.Length > 2000)
        {
            throw new DomainValidationException("Post text is too long.");
        }

        return new Post(userId, text);
    }
    
    public void UpdateText(string? text)
    {
        if (text?.Length > 2000)
        {
            throw new DomainValidationException("Post text is too long.");
        }

        Text = text;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetHiddenStatus(bool mustBeHidden)
    {
        IsHidden = mustBeHidden;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public bool CanUserComment(Guid userId)
    {
        return !IsHidden || UserId == userId;
    }
    
    public bool CanUserAccess(Guid userId)
    {
        if (!IsHidden)
        {
            return true;
        }

        return UserId == userId;
    }
    
    private readonly List<Comment> _comments = [];
    private readonly List<PostLike> _likes = [];
    private readonly List<PostFile> _files = [];
    private readonly List<PostView> _views = [];
}