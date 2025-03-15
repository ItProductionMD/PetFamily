using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Commands.PetManagment.Dtos;
using PetFamily.Application.Commands.VolunteerManagment.GetVolunteers;
using PetFamily.Application.IRepositories;
using PetFamily.Domain.DomainError;
using PetFamily.Domain.PetManagment.Entities;
using PetFamily.Domain.PetManagment.Root;
using PetFamily.Domain.Results;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Infrastructure.Contexts;
using Polly;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace PetFamily.Infrastructure.Repositories.Write;

public class VolunteerRepository(
    ILogger<VolunteerRepository> logger,
    WriteDbContext context) : IVolunteerRepository
{
    private readonly ILogger<VolunteerRepository> _logger = logger;
    private readonly WriteDbContext _context = context;
    public async Task Add(
        Volunteer volunteer,
        CancellationToken cancellationToken = default)
    {
        await _context.Volunteers.AddAsync(volunteer, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result<List<Volunteer>>> GetByEmailOrPhone(
        string email,
        Phone phone,
        CancellationToken cancellation = default)
    {
        List<Volunteer> volunteers = await _context.Volunteers
            .Where(v => v.Email == email ||
            v.PhoneNumber.RegionCode == phone.RegionCode &&
            v.PhoneNumber.Number == phone.Number).ToListAsync(cancellation);

        if (volunteers.Count == 0)
            return Result.Fail(Error.NotFound("Volunteers with souch email or phone"));

        return Result.Ok(volunteers);
    }

    public async Task<Volunteer> GetByIdAsync(Guid id, CancellationToken cancelToken = default)
    {
        var volunteer = await _context.Volunteers
            .Include(v => v.Pets)
            .FirstOrDefaultAsync(v => v.Id == id, cancelToken);

        if (volunteer == null)
        {
            _logger.LogError("Volunteer with id:{id} not found", id);
            throw new KeyNotFoundException($"Volunteer with id {id} not found");
        }
        _logger.LogInformation("Volunteer with id:{id} found", id);
        return volunteer;
    }

    public async Task Save(Volunteer volunteer, CancellationToken cancellToken = default)
    {
        await _context.SaveChangesAsync(cancellToken);
    }
    public async Task Delete(
        Volunteer volunteer,
        CancellationToken cancellToken = default)
    {
        _context.Volunteers.Remove(volunteer);
        await _context.SaveChangesAsync(cancellToken);
    }
    public async Task<UnitResult> UpdateSocialNetworks(
        Guid volunteerId,
        List<SocialNetworkInfo> socialNetworks,
        CancellationToken cancellation = default)
    {
        var socialNetworksValue = await _context.Volunteers
             .Where(v => v.Id == volunteerId)
             .Select(v => v.SocialNetworks)
             .FirstOrDefaultAsync(cancellation);

        if (socialNetworksValue == null)
            return Result.Fail(Error.NotFound("Volunteer"));

        if (socialNetworksValue == socialNetworks)
            return UnitResult.Ok();

        await _context.SaveChangesAsync(cancellation);

        return UnitResult.Ok();
    }

    public async Task SaveWithRetry(Volunteer volunteer, CancellationToken cancelToken = default)
    {
        var retryPolicy = Policy.Handle<Exception>().WaitAndRetryAsync(
               3, retryAttempt => TimeSpan.FromMilliseconds(200 * Math.Pow(2, retryAttempt)),
               onRetry: (exception, timeSpan, retryCount, context) =>
               {
                   _logger.LogWarning($"Attempt {retryCount} failed: {exception.Message}, " +
                       $"retrying in {timeSpan.TotalSeconds}s.");
               });

        await retryPolicy.ExecuteAsync(async () =>
        {
            await Save(volunteer, cancelToken);
        });
    }

    public async Task<VolunteersResponse> GetVolunteersAsync(
        int PageNumber,
        int maxItemsOnPage,
        CancellationToken cancelToken)
    {
        var query = _context.Volunteers.AsNoTracking();

        var countVolunteers = await query.CountAsync(cancelToken);

        var volunteers = await query
            .OrderBy(v => v.Id)
            .Skip((PageNumber - 1) * maxItemsOnPage)
            .Take(maxItemsOnPage)
            .Select(v => new VolunteerDto(
                v.Id,
                v.FullName.FirstName,
                v.FullName.LastName,
                v.Email,
                v.Description ?? "",
                v.PhoneNumber.RegionCode,
                v.PhoneNumber.Number,
                v.ExperienceYears,
                v.PetsForAdoptCount))
            .ToListAsync(cancelToken);

        return new(countVolunteers,volunteers);
    }
}