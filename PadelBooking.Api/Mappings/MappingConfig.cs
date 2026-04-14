using Mapster;
using PadelBooking.Api.Data.Entities;
using PadelBooking.Api.DTOs.Clubs;
using PadelBooking.Api.DTOs.Courts;
using PadelBooking.Api.DTOs.FixedReservations;
using PadelBooking.Api.DTOs.Reservations;

namespace PadelBooking.Api.Mappings;

public class MappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Club Mappings
        config.NewConfig<CreateClubRequest, Club>();
        config.NewConfig<UpdateClubRequest, Club>()
              .IgnoreNullValues(true);
        config.NewConfig<Club, ClubResponse>();

        // Court Mappings
        config.NewConfig<CreateCourtRequest, Court>();
        config.NewConfig<UpdateCourtRequest, Court>()
              .IgnoreNullValues(true);
        config.NewConfig<Court, CourtResponse>();

        // FixedReservation Mappings
        config.NewConfig<CreateFixedReservationRequest, FixedReservation>();
        config.NewConfig<UpdateFixedReservationRequest, FixedReservation>()
              .IgnoreNullValues(true);
        config.NewConfig<FixedReservation, FixedReservationResponse>();

        // Reservation Mappings
        config.NewConfig<CreateReservationRequest, Reservation>();
        config.NewConfig<UpdateReservationRequest, Reservation>()
              .IgnoreNullValues(true);
        config.NewConfig<Reservation, ReservationResponse>();
    }
}
