using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SocialMedia.Persistence.Configurations;

public class ProfilePicConfiguration : IEntityTypeConfiguration<ProfilePic>
{
    public void Configure(EntityTypeBuilder<ProfilePic> builder)
    {
        builder.ToTable("ProfilePics");

        builder.HasKey(pp => pp.Id);

        builder.Property(pp => pp.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(pp => pp.Url)
            .IsRequired();

        builder.Property(pp => pp.ContentType)
            .IsRequired();

        builder.Property(pp => pp.CreatedAt)
            .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

        builder.HasOne(pp => pp.User)
            .WithMany()
            .HasForeignKey(pp => pp.UserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}