using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;

namespace Domain.Entities;

public class User : BaseEntity
{
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Username { get; set; } = "";

    [Required]
    public DateTime BirthDate { get; set; } = DateTime.Now;

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = "";

    [Required]
    [StringLength(255, MinimumLength = 6)]
    public string PasswordHash { get; set; } = "";

    [Url]
    [StringLength(255)]
    public string? ProfilePicUrl { get; set; }

    [StringLength(500)]
    public string? Bio { get; set; }

    [Required]
    public UserStatus Status { get; set; } = UserStatus.Pending;

    public DateTime? LastSeen { get; set; }

    [Required]
    public UserRole UserRole { get; set; } = UserRole.User;

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
    
    public virtual ICollection<Follow> Followees { get; set; } = new List<Follow>();
    
    public virtual ICollection<Follow> Followers { get; set; } = new List<Follow>();
    
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    
    public virtual ICollection<PostLike> PostLikes { get; set; } = new List<PostLike>();
    
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}