using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PadelBooking.Api.Data.Entities;
using PadelBooking.Api.Data.Repositories;
using PadelBooking.Api.DTOs.Reservations;

namespace PadelBooking.Api.Endpoints;

public static class ReservationEndpoints
{
    public static RouteGroupBuilder MapReservationEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetAllReservations);
        group.MapGet("/{id:guid}", GetReservationById);
        group.MapPost("/", CreateReservation);
        group.MapPut("/{id:guid}", UpdateReservation);
        group.MapDelete("/{id:guid}", DeleteReservation);

        return group;
    }

    private static async Task<Ok<IEnumerable<ReservationResponse>>> GetAllReservations(
        IRepository<Reservation> repository,
        CancellationToken cancellationToken)
    {
        var reservations = await repository.GetAllAsync(cancellationToken);
        return TypedResults.Ok(reservations.Adapt<IEnumerable<ReservationResponse>>());
    }

    private static async Task<Results<Ok<ReservationResponse>, NotFound>> GetReservationById(
        Guid id,
        IRepository<Reservation> repository,
        CancellationToken cancellationToken)
    {
        var reservation = await repository.GetByIdAsync(id, cancellationToken);
        return reservation is null ? TypedResults.NotFound() : TypedResults.Ok(reservation.Adapt<ReservationResponse>());
    }

    private static async Task<Results<Created<ReservationResponse>, ValidationProblem>> CreateReservation(
        [FromBody] CreateReservationRequest request,
        IRepository<Reservation> repository,
        IValidator<CreateReservationRequest> validator,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var reservation = request.Adapt<Reservation>();
        await repository.AddAsync(reservation, cancellationToken);
        
        var response = reservation.Adapt<ReservationResponse>();
        return TypedResults.Created($"/api/reservations/{response.Id}", response);
    }

    private static async Task<Results<NoContent, NotFound, ValidationProblem>> UpdateReservation(
        Guid id,
        [FromBody] UpdateReservationRequest request,
        IRepository<Reservation> repository,
        IValidator<UpdateReservationRequest> validator,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var reservation = await repository.GetByIdAsync(id, cancellationToken);
        if (reservation is null)
        {
            return TypedResults.NotFound();
        }

        request.Adapt(reservation);
        await repository.UpdateAsync(reservation, cancellationToken);

        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound>> DeleteReservation(
        Guid id,
        IRepository<Reservation> repository,
        CancellationToken cancellationToken)
    {
        var reservation = await repository.GetByIdAsync(id, cancellationToken);
        if (reservation is null)
        {
            return TypedResults.NotFound();
        }

        await repository.DeleteAsync(reservation, cancellationToken);
        return TypedResults.NoContent();
    }
}
