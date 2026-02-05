using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SocialMedia.Persistence.Configurations;

public class EmailConfirmationTokenConfiguration : IEntityTypeConfiguration<EmailConfirmationToken>
{
    public void Configure(EntityTypeBuilder<EmailConfirmationToken> builder)
    {
        builder.Property(u => u.Version)
            .IsRowVersion();
        
        builder.Property(t => t.Token)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.HasIndex(t => t.Token);
    }
}