using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderDAL.Entity;

namespace OrderDAL.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Order");

        builder.Property(e => e.ItemId)
            .IsRequired();
        
        builder.Property(e => e.UserId)
            .IsRequired();
        
        builder.HasIndex(e => e.Id).IsUnique();

        builder.Property(e => e.CreatedOnUtc)
            .HasColumnType("timestamp with time zone");

        builder.Property(e => e.UpdatedOnUtc)
            .HasColumnType("timestamp with time zone");
    }
}