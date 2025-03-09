using Microsoft.Extensions.Logging;
using PetFamily.Application.FilesManagment;
using PetFamily.Application.FilesManagment.Dtos;
using PetFamily.Application.Species;
using PetFamily.Application.Volunteers;
using PetFamily.Domain.DomainError;
using PetFamily.Domain.PetManagment.Root;
using PetFamily.Domain.Results;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.PetManagment.ValueObjects;
using PetFamily.Domain.PetManagment.Enums;
using PetSpecies = PetFamily.Domain.PetManagment.Entities.Species;
using System;
using System.Reflection.Metadata.Ecma335;
using PetFamily.Domain.PetManagment.Entities;


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
        AddPetCommand command,
        CancellationToken cancelToken = default)
    {
        var validate = AddPetCommandValidator.Validate(command);
        if (validate.IsFailure)
        {
            _logger.LogWarning("Validate add pet command errors:{Errors}", validate.ToErrorMessages());
            return validate;
        }
        var checkPetType = await CheckPetTypeAsync(command.SpeciesId, command.BreedId, cancelToken);
        if (checkPetType.IsFailure)
            return checkPetType;      
        
        var volunteer = await _volunteerRepository.GetByIdAsync(command.VolunteerId, cancelToken);

        var newPet = CreatingPetProccess(command, volunteer);

        await _volunteerRepository.Save(volunteer, cancelToken);

        _logger.LogInformation("Pet with id:{petId} was added to volunteer with id:{Id} successful!",
            newPet.Id, command.VolunteerId);

        return Result.Ok(new AddPetResponse(newPet.Id, newPet.SerialNumber.Value));
    }

    private async Task<UnitResult> CheckPetTypeAsync(Guid speciesId, Guid breedId, CancellationToken cancelToken)
    {
        var species = await _speciesRepository.GetAsync(speciesId, cancelToken);
        if (species == null)
        {
            _logger.LogError("Fail check pet type! Species with id:{Id} not found", speciesId);
            return Result.Fail(Error.NotFound("Species"));
        }
        if (species.Breeds.Any(b => b.Id == breedId) == false)
        {
            _logger.LogError("Fail check pet type! Breed with id:{Id} not found", breedId);
            return Result.Fail(Error.NotFound("Breed"));
        }
        return UnitResult.Ok();
    }

    private Pet CreatingPetProccess(AddPetCommand command, Volunteer volunteer)
    {
        var address = Address.Create(command.Region, command.City, command.Street, command.HomeNumber).Data!;
        var ownerPhone = Phone.Create(command.OwnerPhoneNumber, command.OwnerPhoneRegion).Data!;

        HelpStatus? helpStatus = Enum.IsDefined(typeof(HelpStatus), command.HelpStatus)
            ? (HelpStatus)command.HelpStatus
            : null;

        if (helpStatus == null)
        {
            _logger.LogError("Help status with value:{HelpStatus} not found!" +
                "Set help status to default!", command.HelpStatus);
            helpStatus = HelpStatus.ForAdoption;
        }
        var requisites = command.Requisites
            .Select(d => RequisitesInfo.Create(d.Name, d.Description).Data!).ToList();

        var petType = PetType.Create(
            BreedID.SetValue(command.BreedId), SpeciesID.SetValue(command.SpeciesId)).Data!;

        return volunteer.CreateAndAddPet(
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
            helpStatus.Value,
            command.HealthInfo,
            address);
    }
}