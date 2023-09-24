using InventoryDAL.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryDAL.Configurations;

public class InventoryConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.ToTable("Users");

        builder.Property(e => e.Name)
            .IsRequired();
        
        builder.HasIndex(e => e.Id).IsUnique();

        builder.Property(e => e.CreatedOnUtc)
            .HasColumnType("timestamp with time zone");

        builder.Property(e => e.UpdatedOnUtc)
            .HasColumnType("timestamp with time zone");
    }
}