using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PadelBooking.Api.Data.Entities;
using PadelBooking.Api.Data.Repositories;
using PadelBooking.Api.DTOs.FixedReservations;

namespace PadelBooking.Api.Endpoints;

public static class FixedReservationEndpoints
{
    public static RouteGroupBuilder MapFixedReservationEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetAllFixedReservations);
        group.MapGet("/{id:guid}", GetFixedReservationById);
        group.MapPost("/", CreateFixedReservation);
        group.MapPut("/{id:guid}", UpdateFixedReservation);
        group.MapDelete("/{id:guid}", DeleteFixedReservation);

        return group;
    }

    private static async Task<Ok<IEnumerable<FixedReservationResponse>>> GetAllFixedReservations(
        IRepository<FixedReservation> repository,
        CancellationToken cancellationToken)
    {
        var reservations = await repository.GetAllAsync(cancellationToken);
        return TypedResults.Ok(reservations.Adapt<IEnumerable<FixedReservationResponse>>());
    }

    private static async Task<Results<Ok<FixedReservationResponse>, NotFound>> GetFixedReservationById(
        Guid id,
        IRepository<FixedReservation> repository,
        CancellationToken cancellationToken)
    {
        var reservation = await repository.GetByIdAsync(id, cancellationToken);
        return reservation is null ? TypedResults.NotFound() : TypedResults.Ok(reservation.Adapt<FixedReservationResponse>());
    }

    private static async Task<Results<Created<FixedReservationResponse>, ValidationProblem>> CreateFixedReservation(
        [FromBody] CreateFixedReservationRequest request,
        IRepository<FixedReservation> repository,
        IValidator<CreateFixedReservationRequest> validator,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var reservation = request.Adapt<FixedReservation>();
        await repository.AddAsync(reservation, cancellationToken);
        
        var response = reservation.Adapt<FixedReservationResponse>();
        return TypedResults.Created($"/api/fixed-reservations/{response.Id}", response);
    }

    private static async Task<Results<NoContent, NotFound, ValidationProblem>> UpdateFixedReservation(
        Guid id,
        [FromBody] UpdateFixedReservationRequest request,
        IRepository<FixedReservation> repository,
        IValidator<UpdateFixedReservationRequest> validator,
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

    private static async Task<Results<NoContent, NotFound>> DeleteFixedReservation(
        Guid id,
        IRepository<FixedReservation> repository,
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
