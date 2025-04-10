using System;
using System.Collections.Generic;

namespace Infrastructure;

public partial class Post
{
    public int PostId { get; set; }

    public string? Text { get; set; }

    public int? UserId { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual User? User { get; set; }
}
