using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects;
using PetSpecies.Public.IContracts;
using Volunteers.Application.IRepositories;
using Volunteers.Domain;
using Volunteers.Domain.Enums;

namespace Volunteers.Application.Commands.PetManagement.UpdatePet;

public class UpdatePetHandler(
    IVolunteerWriteRepository volunteerWriteRepo,
    ISpeciesExistenceContract petTypeChecker,
    ILogger<UpdatePetHandler> logger) : ICommandHandler<UpdatePetCommand>
{

    public async Task<UnitResult> Handle(UpdatePetCommand cmd, CancellationToken cancelToken)
    {
        cmd.Validate();

        var getVolunteer = await volunteerWriteRepo.GetByIdAsync(cmd.VolunteerId, cancelToken);
        if (getVolunteer.IsFailure)
            return UnitResult.Fail(getVolunteer.Error);

        var volunteer = getVolunteer.Data!;

        var checkPetType = await petTypeChecker.VerifySpeciesAndBreedExist(
            cmd.SpeciesId,
            cmd.BreedId,
            cancelToken);
        if (checkPetType.IsFailure)
            return checkPetType;

        var updatePetResult = UpdatePetProcess(volunteer, cmd);
        if (updatePetResult.IsFailure)
        {

            logger.LogWarning("Update pet with id:{ id} error:{error}!",
                cmd.PetId, updatePetResult.ToErrorMessage());
            return updatePetResult;
        }

        var result = await volunteerWriteRepo.SaveAsync(volunteer, cancelToken);
        if (result.IsFailure)
            return result;

        logger.LogInformation("Update pet with id:{id} successful !", cmd.PetId);

        return UnitResult.Ok();
    }
    private UnitResult UpdatePetProcess(Volunteer volunteer, UpdatePetCommand cmd)
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

        return volunteer.UpdatePet(
            cmd.PetId,
            new(
                cmd.PetName,
                cmd.DateOfBirth,
                cmd.Description,
                cmd.IsVaccinated,
                cmd.IsSterilized,
                cmd.Weight,
                cmd.Height,
                cmd.Color,
                petType,
                phone,
                requisites,
                helpStatus,
                cmd.HealthInfo,
                address)
            );
    }
}
