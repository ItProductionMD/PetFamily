using Microsoft.Extensions.Logging;
using PetFamily.Application.FilesManagment;
using PetFamily.Application.FilesManagment.Dtos;
using PetFamily.Application.Species;
using PetFamily.Application.Volunteers;
using PetFamily.Domain.DomainError;
using PetFamily.Domain.PetAggregates.Enums;
using PetFamily.Domain.PetAggregates.Root;
using PetFamily.Domain.PetAggregates.ValueObjects;
using PetFamily.Domain.Results;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.Dtos;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.VolunteerAggregates.Root;
using System.Threading;
using static PetFamily.Domain.Shared.ValueObjects.ValueObjectFactory;
using PetSpecies = PetFamily.Domain.PetAggregates.Entities.Species;


namespace PetFamily.Application.Pets.CreatePet;

public class AddPetHandler(
    ILogger<AddPetHandler> logger,
    IVolunteerRepository volunteerRepository,
    IPetRepository petRepository,
    ISpeciesRepository speciesRepository)
{
    private readonly ISpeciesRepository _speciesRepository = speciesRepository;
    private readonly ILogger<AddPetHandler> _logger = logger;
    private readonly IVolunteerRepository _volunteerRepository = volunteerRepository;
    private readonly IPetRepository _petRepository = petRepository;

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
        var createPetResult = Pet.Create(CreatePetDomainDto(command));
        if (createPetResult.IsFailure)
        {
            _logger.LogError("Fail create pet while adding a new pet!Errors:{Errors}",
                createPetResult.ConcateErrorMessages());

            return Result.Fail(createPetResult.Errors!);
        }

        var pet = createPetResult.Data!;
        //----------------------------------Get Volunteer-------------------------------------//
        var getVolunteer = await _volunteerRepository.GetById(volunteerId, cancellationToken);
        if (getVolunteer.IsFailure)
        {
            _logger.LogError("Get volunteer with Id:{volunteerId} errors:{Errors}",
                volunteerId, getVolunteer.ConcateErrorMessages());

            return Result.Fail(getVolunteer.Errors!);
        }
        var volunteer = getVolunteer.Data!;

        volunteer.AddPet(pet);
        //--------------------------------Save Pet in DB----------------------------------//
        var saveResult = await _petRepository.AddAsync(volunteer, pet);
        if (saveResult.IsFailure)
        {
            _logger.LogError("Save volunteer errors!{error}",saveResult.ConcateErrorMessages());
            return Result.Fail(saveResult.Errors!);
        }      
        var addPetResponse = new AddPetResponse(pet.Id,pet.SerialNumber.Value);

        return Result.Ok(addPetResponse);
    }
    //---------------------------------Private methods--------------------------------------------//
    private static PetDomainDto CreatePetDomainDto(AddPetCommand command)
    {
        var address = Address.Create(command.Region, command.City, command.Street, command.HomeNumber).Data!;

        var ownerPhone = Phone.Create(command.OwnerPhoneNumber, command.OwnerPhoneRegion).Data!;

        var donateDetails = MapDtosToValueObjects(
            command.DonateDetails, dto => RequisitesInfo.Create(dto.Name, dto.Description))!;

        var petType = PetType.Create(
            BreedID.SetValue(command.BreedId), SpeciesID.SetValue(command.SpeciesId)).Data!;

        return new PetDomainDto(
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
            donateDetails,
            command.HealthInfo,
            address,
            (HelpStatus)command.HelpStatus, []);
    }

    private  async Task<UnitResult> CheckPetTypeAsync(
        Guid speciesId,
        Guid breedId,
        CancellationToken cancellationToken)
    {
        var species = await _speciesRepository.GetAsync(speciesId, cancellationToken);
        if (species == null)
        {
            _logger.LogError("Species with id:{speciesId} not found",speciesId);
            return UnitResult.Fail(Error.NotFound("Species"));
        }
        if (species.Breeds.Any(b => b.Id == breedId) == false)
        {
            _logger.LogError("Breed with id:{breedId} not found", breedId);
            return UnitResult.Fail(Error.NotFound("Breed"));
        }

        return UnitResult.Ok();
    }

}