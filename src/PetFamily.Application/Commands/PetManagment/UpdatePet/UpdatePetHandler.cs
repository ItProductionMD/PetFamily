
using Microsoft.Extensions.Logging;
using PetFamily.Application.Abstractions;
using PetFamily.Application.IRepositories;
using PetFamily.Domain.DomainError;
using PetFamily.Domain.PetManagment.Entities;
using PetFamily.Domain.PetManagment.Enums;
using PetFamily.Domain.PetManagment.ValueObjects;
using PetFamily.Domain.Results;
using PetFamily.Domain.Shared.ValueObjects;
using Polly;
using System.ComponentModel.DataAnnotations;
using static PetFamily.Application.Commands.PetManagment.UpdatePet.UpdatePetCommandValidator;

namespace PetFamily.Application.Commands.PetManagment.UpdatePet;

public class UpdatePetHandler(
    IVolunteerWriteRepository volunteerWriteRepository,
    ISpeciesReadRepository speciesReadRepository,
    ILogger<UpdatePetHandler> logger) : ICommandHandler<UpdatePetCommand>
{
    private readonly IVolunteerWriteRepository _writeRepository = volunteerWriteRepository;
    private readonly ILogger<UpdatePetHandler> _logger = logger;
    private readonly ISpeciesReadRepository _speciesReadRepository = speciesReadRepository;
    
    public async Task<UnitResult> Handle(UpdatePetCommand cmd, CancellationToken cancelToken)
    {
        var validateCommand = Validate(cmd);
        if(validateCommand.IsFailure)
        {
            _logger.LogWarning("Update pet with id:{Id} validation errors:{Errors}",
                cmd.PetId, validateCommand.ValidationMessagesToString());
            return validateCommand;
        }

        var getVolunteer = await _writeRepository.GetByIdAsync(cmd.VolunteerId, cancelToken);
        if (getVolunteer.IsFailure)
            return UnitResult.Fail(getVolunteer.Error);

        var volunteer = getVolunteer.Data!;

        var pet = volunteer.Pets.FirstOrDefault(p => p.Id == cmd.PetId);
        if (pet == null)
        {
            _logger.LogWarning("Pet with id:{Id} not found", cmd.PetId);
            return UnitResult.Fail(Error.NotFound($"Pet with id:{cmd.PetId}"));
        }

        var checkPetType = await _speciesReadRepository.CheckIfPetTypeExists(
            cmd.SpeciesId,
            cmd.BreedId, 
            cancelToken);
        if (checkPetType.IsFailure)
            return checkPetType;

        UpdatePetProccess(pet, cmd);

        var result = await _writeRepository.Save(volunteer, cancelToken);
        if (result.IsFailure)
            return result;

        _logger.LogInformation("Update pet with id:{id} soccessful!", pet.Id);

        return UnitResult.Ok();    
    }
    private void UpdatePetProccess(Pet pet,UpdatePetCommand cmd)
    {
        var petType = PetType.Create(
            BreedID.SetValue(cmd.BreedId),
            SpeciesID.SetValue(cmd.SpeciesId)).Data!;

        var phone = Phone.CreatePossibbleEmpty(cmd.PhoneNumber, cmd.PhoneRegion).Data!;

        var requisites = cmd.Requisites
            .Select(r => RequisitesInfo.Create(r.Name, r.Description).Data!)
            .ToList();

        var address = Address.CreatePossibleEmpty(cmd.Region, cmd.City, cmd.Street, cmd.HomeNumber).Data!;

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
