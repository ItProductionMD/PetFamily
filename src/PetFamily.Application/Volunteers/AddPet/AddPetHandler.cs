using Microsoft.Extensions.Logging;
using PetFamily.Application.FilesManagment;
using PetFamily.Application.FilesManagment.Dtos;
using PetFamily.Application.Species;
using PetFamily.Application.Volunteers;
using PetFamily.Domain.DomainError;
using PetFamily.Domain.PetManagment.Root;
using PetFamily.Domain.Results;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.Dtos;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.PetManagment.ValueObjects;
using PetFamily.Domain.PetManagment.Enums;
using PetSpecies = PetFamily.Domain.PetManagment.Entities.Species;
using System;
using System.Reflection.Metadata.Ecma335;


namespace PetFamily.Application.Volunteers.AddPet;

public class AddPetHandler(
    ILogger<AddPetHandler> logger,
    IVolunteerRepository volunteerRepository,
    ISpeciesRepository speciesRepository)
{
    private readonly ISpeciesRepository _speciesRepository = speciesRepository;
    private readonly ILogger<AddPetHandler> _logger = logger;
    private readonly IVolunteerRepository _volunteerRepository = volunteerRepository;

    public async Task<Result<AddPetResponse>> Handle(
        Guid volunteerId,
        AddPetCommand command,
        CancellationToken cancellationToken = default)
    {
        //-------------------------------------Validation-------------------------------------//
        var validationResult = AddPetCommandValidator.Validate(command);
        if (validationResult.IsFailure)
        {
            _logger.LogWarning("Validate add pet command errors:{Errors}",
                validationResult.ConcateErrorMessages());

            return validationResult;
        }
        //-----------------------------Check if Species and Breed exists----------------------//
        var checkPetTypeResult = await CheckPetTypeAsync(
            command.SpeciesId,
            command.BreedId,
            cancellationToken);

        if (checkPetTypeResult.IsFailure)
        {
            _logger.LogError("Fail check pet type while adding a new pet!Errors:{Errors}",
                checkPetTypeResult.ConcateErrorMessages());

            return checkPetTypeResult;
        }
        //----------------------------------Get Volunteer-------------------------------------//
        var getVolunteer = await _volunteerRepository.GetByIdAsync(volunteerId, cancellationToken);
        if (getVolunteer.IsFailure)
        {
            _logger.LogError("Get volunteer with Id:{volunteerId} errors:{Errors}",
                volunteerId, getVolunteer.ConcateErrorMessages());

            return Result.Fail(getVolunteer.Errors!);
        }
        var volunteer = getVolunteer.Data!;

        var address = Address.Create(command.Region, command.City, command.Street, command.HomeNumber).Data!;

        var ownerPhone = Phone.Create(command.OwnerPhoneNumber, command.OwnerPhoneRegion).Data!;

        HelpStatus? helpStatus = Enum.IsDefined(typeof(HelpStatus), command.HelpStatus)
            ? (HelpStatus)command.HelpStatus
            : null;

        if (helpStatus == null)
        {
            _logger.LogError("Help status with value:{HelpStatus} not found!", command.HelpStatus);
            return Result.Fail(Error.NotFound("HelpStatus"));
        }

        var requisites = command.Requisites
            .Select(d => RequisitesInfo.Create(d.Name, d.Description).Data!).ToList();

        var petType = PetType.Create(
            BreedID.SetValue(command.BreedId), SpeciesID.SetValue(command.SpeciesId)).Data!;

        var newPet = volunteer.CreateAndAddPet(
            command.PetName,
            command.DateOfBirth,
            command.Description,
            command.IsVaccinated,
            command.IsSterilized,
            command.Weight,
            command.Height,
            command.Color,
            petType,
            ownerPhone,
            requisites,
            [],
            helpStatus.Value,
            command.HealthInfo,
            address);

        await _volunteerRepository.Save(volunteer, cancellationToken);

        var addPetResponse = new AddPetResponse(newPet.Id, newPet.SerialNumber.Value);

        _logger.LogInformation("Pet with id:{petId} was added to volunteer with id:{Id} successful!",
            newPet.Id, volunteerId);

        return Result.Ok(addPetResponse);
    }
    //---------------------------------Private methods--------------------------------------------//

    private async Task<UnitResult> CheckPetTypeAsync(
        Guid speciesId,
        Guid breedId,
        CancellationToken cancellationToken)
    {
        var species = await _speciesRepository.GetAsync(speciesId, cancellationToken);
        if (species == null)
        {
            _logger.LogError("Species with id:{speciesId} not found", speciesId);
            return Result.Fail(Error.NotFound("Species"));
        }
        if (species.Breeds.Any(b => b.Id == breedId) == false)
        {
            _logger.LogError("Breed with id:{breedId} not found", breedId);
            return Result.Fail(Error.NotFound("Breed"));
        }

        return UnitResult.Ok();
    }

}