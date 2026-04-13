using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PadelBooking.Api.Data.Entities;

namespace PadelBooking.Api.Data.Configurations;

public class CourtConfiguration : IEntityTypeConfiguration<Court>
{
    public void Configure(EntityTypeBuilder<Court> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasDefaultValueSql("NEWID()");

        builder.Property(c => c.Name).IsRequired().HasMaxLength(50);
        builder.Property(c => c.SurfaceType).HasMaxLength(50);
        builder.Property(c => c.IsCovered).HasDefaultValue(false);
        builder.Property(c => c.IsActive).HasDefaultValue(true);
        builder.Property(c => c.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(c => c.Club)
            .WithMany(cl => cl.Courts)
            .HasForeignKey(c => c.ClubId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Reservations)
            .WithOne(r => r.Court)
            .HasForeignKey(r => r.CourtId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.FixedReservations)
            .WithOne(fr => fr.Court)
            .HasForeignKey(fr => fr.CourtId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
