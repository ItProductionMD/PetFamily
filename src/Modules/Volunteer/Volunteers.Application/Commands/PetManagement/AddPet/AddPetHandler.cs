using Microsoft.Extensions.Logging;
using PetFamily.Application.Abstractions.CQRS;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects;
using PetSpecies.Public.IContracts;
using Volunteers.Application.IRepositories;
using Volunteers.Domain;
using Volunteers.Domain.Enums;


namespace Volunteers.Application.Commands.PetManagement.AddPet;

public class AddPetHandler(
    ILogger<AddPetHandler> logger,
    IVolunteerWriteRepository repository,
    ISpeciesExistenceContract petTypeChecker) : ICommandHandler<AddPetResponse, AddPetCommand>
{
    private readonly ILogger<AddPetHandler> _logger = logger;
    private readonly IVolunteerWriteRepository _repository = repository;
    private readonly ISpeciesExistenceContract _petTypeChecker = petTypeChecker;

    public async Task<Result<AddPetResponse>> Handle(
        AddPetCommand command,
        CancellationToken cancelToken = default)
    {
        var validate = AddPetCommandValidator.Validate(command);
        if (validate.IsFailure)
        {
            _logger.LogWarning("Validate add pet command errors:{Errors}",
                validate.ValidationMessagesToString());

            return validate;
        }

        var checkPetType = await _petTypeChecker.VerifySpeciesAndBreedExist(
            command.SpeciesId,
            command.BreedId,
            cancelToken);
        if (checkPetType.IsFailure)
            return checkPetType;

        var getVolunteer = await _repository.GetByIdAsync(command.VolunteerId, cancelToken);
        if (getVolunteer.IsFailure)
            return UnitResult.Fail(getVolunteer.Error);

        var volunteer = getVolunteer.Data!;

        var newPet = CreatePetProcess(command, volunteer);

        var result = await _repository.Save(volunteer, cancelToken);
        if (result.IsFailure)
            return result;

        _logger.LogInformation("Pet with id:{petId} was added to volunteer with id:{Id} successful!",
            newPet.Id, command.VolunteerId);

        return Result.Ok(new AddPetResponse(newPet.Id, newPet.SerialNumber.Value));
    }

    private static Pet CreatePetProcess(AddPetCommand cmd, Volunteers.Domain.Volunteer volunteer)
    {
        var address = Address.CreatePossibleEmpty(cmd.Region, cmd.City, cmd.Street, cmd.HomeNumber).Data!;

        var ownerPhone = Phone.CreatePossibbleEmpty(cmd.OwnerPhoneNumber, cmd.OwnerPhoneRegion).Data!;

        var helpStatus = (HelpStatus)cmd.HelpStatus;

        var requisites = cmd.Requisites
            .Select(d => RequisitesInfo.Create(d.Name, d.Description).Data!).ToList();

        var petType = PetType.Create(
            BreedID.SetValue(cmd.BreedId), SpeciesID.SetValue(cmd.SpeciesId)).Data!;

        return volunteer.CreateAndAddPet(
            cmd.PetName,
            cmd.DateOfBirth,
            cmd.Description,
            cmd.IsVaccinated,
            cmd.IsSterilized,
            cmd.Weight,
            cmd.Height,
            cmd.Color,
            petType,
            ownerPhone,
            requisites,
            helpStatus,
            cmd.HealthInfo,
            address);
    }
}