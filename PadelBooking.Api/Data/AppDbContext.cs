using System.Reflection;
using Microsoft.EntityFrameworkCore;
using PadelBooking.Api.Data.Entities;
using PadelBooking.Api.Data.Enums;
using PadelBooking.Api.Security;

namespace PadelBooking.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options, IUserContext? userContext = null) : DbContext(options)
{
    public DbSet<Club> Clubs => Set<Club>();
    public DbSet<Court> Courts => Set<Court>();
    public DbSet<FixedReservation> FixedReservations => Set<FixedReservation>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<User> Users => Set<User>();

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<AuditableEntity>();
        var currentUserId = userContext?.CurrentUserId;

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
                entry.Entity.CreatedBy = currentUserId;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.LastModifiedAt = DateTime.UtcNow;
                entry.Entity.LastModifiedBy = currentUserId;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        var clubId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        
        var adminId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var ownerId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = adminId,
                ExternalAuthId = "admin-keycloak-id",
                Email = "admin@padelbooking.com",
                FirstName = "System",
                LastName = "Admin",
                Role = UserRole.Admin,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new User
            {
                Id = ownerId,
                ExternalAuthId = "owner-keycloak-id",
                Email = "owner@padelbooking.com",
                FirstName = "Padel Pro",
                LastName = "Owner",
                Role = UserRole.Owner,
                ClubId = clubId,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
        
        modelBuilder.Entity<Club>().HasData(new Club
        {
            Id = clubId,
            Name = "Padel Pro Club",
            Address = "123 Padel St, Sports City",
            OpenTime = new TimeOnly(8, 0),
            CloseTime = new TimeOnly(23, 0),
            IsActive = true,
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });

        modelBuilder.Entity<Court>().HasData(
            new Court
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                ClubId = clubId,
                Name = "Court 1 (Glass)",
                SurfaceType = "Artificial Grass",
                IsCovered = true,
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                LayoutPositionX = 0,
                LayoutPositionY = 0,
                LayoutRotationZ = 0
            },
            new Court
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                ClubId = clubId,
                Name = "Court 2 (WPT)",
                SurfaceType = "Mondo Supercourt",
                IsCovered = false,
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                LayoutPositionX = 15,
                LayoutPositionY = 0,
                LayoutRotationZ = 0
            }
        );
    }
}
