using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PadelBooking.Api.Data.Entities;

namespace PadelBooking.Api.Data.Configurations;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).HasDefaultValueSql("NEWID()");

        builder.Property(r => r.CustomerName).IsRequired().HasMaxLength(100);
        builder.Property(r => r.CustomerPhone).IsRequired().HasMaxLength(50);
        builder.Property(r => r.ReservationDate).IsRequired();
        builder.Property(r => r.StartTime).IsRequired();
        builder.Property(r => r.EndTime).IsRequired();

        builder.Property(r => r.TotalAmount).IsRequired().HasColumnType("decimal(10,2)");
        builder.Property(r => r.DepositAmount).HasColumnType("decimal(10,2)").HasDefaultValue(0m);
        builder.Property(r => r.IsDepositPaid).HasDefaultValue(false);
        builder.Property(r => r.PaymentMethod).HasMaxLength(50);
        builder.Property(r => r.Status).IsRequired().HasMaxLength(20).HasDefaultValue("Confirmed");

        builder.Property(r => r.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(r => r.Court)
            .WithMany(c => c.Reservations)
            .HasForeignKey(r => r.CourtId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.FixedReservation)
            .WithMany(fr => fr.Reservations)
            .HasForeignKey(r => r.FixedReservationId)
            .OnDelete(DeleteBehavior.SetNull);

        // Index for availability queries
        builder.HasIndex(r => new { r.ReservationDate, r.StartTime, r.EndTime })
            .HasDatabaseName("IX_Reservations_Date_Times");
    }
}
