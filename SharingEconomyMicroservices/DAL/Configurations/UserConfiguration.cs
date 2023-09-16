using DAL.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        
        builder.Property(e => e.CreatedOnUtc)
            .HasColumnType("timestamp with time zone");
        
        builder.Property(e => e.UpdatedOnUtc)
            .HasColumnType("timestamp with time zone");
    }
}
