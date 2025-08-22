using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects;
using PetSpecies.Public.IContracts;
using Volunteers.Application.IRepositories;
using Volunteers.Domain;
using Volunteers.Domain.Enums;


namespace Volunteers.Application.Commands.PetManagement.AddPet;

public class AddPetHandler(
    IVolunteerWriteRepository volunteerWriteRepo,
    ISpeciesExistenceContract petTypeChecker,
    ILogger<AddPetHandler> logger) : ICommandHandler<AddPetResponse, AddPetCommand>
{
    public async Task<Result<AddPetResponse>> Handle(AddPetCommand cmd, CancellationToken ct = default)
    {
        cmd.Validate();

        var checkPetType = await petTypeChecker.VerifySpeciesAndBreedExist(
            cmd.SpeciesId,
            cmd.BreedId,
            ct);
        if (checkPetType.IsFailure)
            return checkPetType;

        var getVolunteer = await volunteerWriteRepo.GetByIdAsync(cmd.VolunteerId, ct);
        if (getVolunteer.IsFailure)
            return UnitResult.Fail(getVolunteer.Error);

        var volunteer = getVolunteer.Data!;

        var newPet = CreatePetProcess(cmd, volunteer);

        var result = await volunteerWriteRepo.SaveAsync(volunteer, ct);
        if (result.IsFailure)
            return result;

        logger.LogInformation("Pet with id:{petId} was added to volunteer with id:{Id} successful!",
            newPet.Id, cmd.VolunteerId);

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