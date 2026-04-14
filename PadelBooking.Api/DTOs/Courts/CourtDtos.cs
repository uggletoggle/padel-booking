namespace PadelBooking.Api.DTOs.Courts;

public record CreateCourtRequest(
    Guid ClubId,
    string Name,
    string? SurfaceType,
    bool IsCovered,
    double? LayoutPositionX,
    double? LayoutPositionY,
    double? LayoutRotationZ
);

public record UpdateCourtRequest(
    string Name,
    string? SurfaceType,
    bool IsCovered,
    double? LayoutPositionX,
    double? LayoutPositionY,
    double? LayoutRotationZ,
    bool IsActive
);

public record CourtResponse(
    Guid Id,
    Guid ClubId,
    string Name,
    string? SurfaceType,
    bool IsCovered,
    double? LayoutPositionX,
    double? LayoutPositionY,
    double? LayoutRotationZ,
    bool IsActive,
    DateTime CreatedAt
);
