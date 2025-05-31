using Microsoft.Extensions.Logging;
using PetFamily.Application.Abstractions.CQRS;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects;
using PetSpecies.Public.IContracts;
using Volunteers.Application.IRepositories;
using Volunteers.Domain;
using Volunteers.Domain.Enums;

namespace Volunteers.Application.Commands.PetManagement.UpdatePet;

public class UpdatePetHandler(
    IVolunteerWriteRepository volunteerWriteRepository,
    ISpeciesExistenceContract speciesReadRepository,
    ILogger<UpdatePetHandler> logger) : ICommandHandler<UpdatePetCommand>
{
    private readonly IVolunteerWriteRepository _writeRepository = volunteerWriteRepository;
    private readonly ILogger<UpdatePetHandler> _logger = logger;
    private readonly ISpeciesExistenceContract _petTypeChecker = speciesReadRepository;

    public async Task<UnitResult> Handle(UpdatePetCommand cmd, CancellationToken cancelToken)
    {
        var validationResult = UpdatePetCommandValidator.Validate(cmd);
        if (validationResult.IsFailure)
        {
            _logger.LogWarning("Update pet with id:{Id} validation errors:{Errors}",
                cmd.PetId, validationResult.ValidationMessagesToString());

            return validationResult;
        }

        var getVolunteer = await _writeRepository.GetByIdAsync(cmd.VolunteerId, cancelToken);
        if (getVolunteer.IsFailure)
            return UnitResult.Fail(getVolunteer.Error);

        Volunteers.Domain.Volunteer volunteer = getVolunteer.Data!;

        var pet = volunteer.Pets.FirstOrDefault(p => p.Id == cmd.PetId);
        if (pet == null)
        {
            _logger.LogWarning("Pet with id:{Id} not found", cmd.PetId);
            return UnitResult.Fail(Error.NotFound($"Pet with id:{cmd.PetId}"));
        }

        var checkPetType = await _petTypeChecker.VerifySpeciesAndBreedExist(
            cmd.SpeciesId,
            cmd.BreedId,
            cancelToken);
        if (checkPetType.IsFailure)
            return checkPetType;

        UpdatePetProcess(pet, cmd);

        var result = await _writeRepository.Save(volunteer, cancelToken);
        if (result.IsFailure)
            return result;

        _logger.LogInformation("Update pet with id:{id} successful !", pet.Id);

        return UnitResult.Ok();
    }
    private void UpdatePetProcess(Pet pet, UpdatePetCommand cmd)
    {
        var petType = PetType.Create(
            BreedID.SetValue(cmd.BreedId),
            SpeciesID.SetValue(cmd.SpeciesId)).Data!;

        var phone = Phone.CreatePossibbleEmpty(cmd.PhoneNumber, cmd.PhoneRegion).Data!;

        var requisites = cmd.Requisites
            .Select(r => RequisitesInfo.Create(r.Name, r.Description).Data!)
            .ToList();

        var address = Address.CreatePossibleEmpty(
            cmd.Region,
            cmd.City,
            cmd.Street,
            cmd.HomeNumber).Data!;

        var helpStatus = (HelpStatus)cmd.HelpStatus;

        pet.Update(
            name: cmd.PetName,
            dateOfBirth: cmd.DateOfBirth,
            description: cmd.Description,
            isVaccinated: cmd.IsVaccinated,
            isSterilized: cmd.IsSterilized,
            weight: cmd.Weight,
            height: cmd.Height,
            color: cmd.Color,
            petType: petType,
            ownerPhone: phone,
            requisites: requisites,
            helpStatus: helpStatus,
            healthInfo: cmd.HealthInfo,
            address: address);
    }
}
