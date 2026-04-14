using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PadelBooking.Api.Data.Entities;
using PadelBooking.Api.Data.Repositories;
using PadelBooking.Api.DTOs.Courts;

namespace PadelBooking.Api.Endpoints;

public static class CourtEndpoints
{
    public static RouteGroupBuilder MapCourtEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetAllCourts);
        group.MapGet("/{id:guid}", GetCourtById);
        group.MapPost("/", CreateCourt);
        group.MapPut("/{id:guid}", UpdateCourt);
        group.MapDelete("/{id:guid}", DeleteCourt);

        return group;
    }

    private static async Task<Ok<IEnumerable<CourtResponse>>> GetAllCourts(
        IRepository<Court> repository,
        CancellationToken cancellationToken)
    {
        var courts = await repository.GetAllAsync(cancellationToken);
        return TypedResults.Ok(courts.Adapt<IEnumerable<CourtResponse>>());
    }

    private static async Task<Results<Ok<CourtResponse>, NotFound>> GetCourtById(
        Guid id,
        IRepository<Court> repository,
        CancellationToken cancellationToken)
    {
        var court = await repository.GetByIdAsync(id, cancellationToken);
        return court is null ? TypedResults.NotFound() : TypedResults.Ok(court.Adapt<CourtResponse>());
    }

    private static async Task<Results<Created<CourtResponse>, ValidationProblem>> CreateCourt(
        [FromBody] CreateCourtRequest request,
        IRepository<Court> repository,
        IValidator<CreateCourtRequest> validator,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var court = request.Adapt<Court>();
        await repository.AddAsync(court, cancellationToken);
        
        var response = court.Adapt<CourtResponse>();
        return TypedResults.Created($"/api/courts/{response.Id}", response);
    }

    private static async Task<Results<NoContent, NotFound, ValidationProblem>> UpdateCourt(
        Guid id,
        [FromBody] UpdateCourtRequest request,
        IRepository<Court> repository,
        IValidator<UpdateCourtRequest> validator,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var court = await repository.GetByIdAsync(id, cancellationToken);
        if (court is null)
        {
            return TypedResults.NotFound();
        }

        request.Adapt(court);
        await repository.UpdateAsync(court, cancellationToken);

        return TypedResults.NoContent();
    }

    private static async Task<Results<NoContent, NotFound>> DeleteCourt(
        Guid id,
        IRepository<Court> repository,
        CancellationToken cancellationToken)
    {
        var court = await repository.GetByIdAsync(id, cancellationToken);
        if (court is null)
        {
            return TypedResults.NotFound();
        }

        await repository.DeleteAsync(court, cancellationToken);
        return TypedResults.NoContent();
    }
}
