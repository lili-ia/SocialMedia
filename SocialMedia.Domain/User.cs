using System;
using System.Collections.Generic;

namespace Infrastructure;

public partial class User
{
    public int UserId { get; set; }

    public string? Username { get; set; }

    public DateTime? BirthDate { get; set; }

    public string? Email { get; set; }

    public string? PasswordHash { get; set; }

    public string? ProfilePicUrl { get; set; }

    public string? Bio { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
}
