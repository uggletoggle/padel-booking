using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PadelBooking.Api.Data.Entities;

namespace PadelBooking.Api.Data.Configurations;

public class FixedReservationConfiguration : IEntityTypeConfiguration<FixedReservation>
{
    public void Configure(EntityTypeBuilder<FixedReservation> builder)
    {
        builder.HasKey(fr => fr.Id);
        builder.Property(fr => fr.Id).HasDefaultValueSql("NEWID()");

        builder.Property(fr => fr.CustomerName).IsRequired().HasMaxLength(100);
        builder.Property(fr => fr.CustomerPhone).IsRequired().HasMaxLength(50);
        builder.Property(fr => fr.DayOfWeek).IsRequired();
        builder.Property(fr => fr.StartTime).IsRequired();
        builder.Property(fr => fr.EndTime).IsRequired();
        builder.Property(fr => fr.StartDate).IsRequired();
        builder.Property(fr => fr.IsActive).HasDefaultValue(true);
        builder.Property(fr => fr.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(fr => fr.Court)
            .WithMany(c => c.FixedReservations)
            .HasForeignKey(fr => fr.CourtId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(fr => fr.Reservations)
            .WithOne(r => r.FixedReservation)
            .HasForeignKey(r => r.FixedReservationId)
            .OnDelete(DeleteBehavior.SetNull);

        // Index for availability queries
        builder.HasIndex(fr => new { fr.DayOfWeek, fr.StartTime, fr.EndTime })
            .HasDatabaseName("IX_FixedReservations_Day_Times");
    }
}
