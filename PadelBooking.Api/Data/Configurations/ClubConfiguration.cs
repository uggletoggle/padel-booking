using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PadelBooking.Api.Data.Entities;

namespace PadelBooking.Api.Data.Configurations;

public class ClubConfiguration : IEntityTypeConfiguration<Club>
{
    public void Configure(EntityTypeBuilder<Club> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasDefaultValueSql("NEWID()");

        builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Address).HasMaxLength(200);
        builder.Property(c => c.OpenTime).IsRequired();
        builder.Property(c => c.CloseTime).IsRequired();
        builder.Property(c => c.IsActive).HasDefaultValue(true);
        builder.Property(c => c.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

        builder.HasMany(c => c.Courts)
            .WithOne(ct => ct.Club)
            .HasForeignKey(ct => ct.ClubId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
