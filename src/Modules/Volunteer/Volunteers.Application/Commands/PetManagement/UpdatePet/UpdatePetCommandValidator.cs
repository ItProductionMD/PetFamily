using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects;
using Volunteers.Domain.Enums;
using static PetFamily.SharedKernel.Validations.ValidationConstants;
using static PetFamily.SharedKernel.Validations.ValidationExtensions;
using static PetFamily.SharedKernel.Validations.ValidationPatterns;

namespace Volunteers.Application.Commands.PetManagement.UpdatePet;

public static class UpdatePetCommandValidator
{
    public static UnitResult Validate(UpdatePetCommand command)
    {
        return UnitResult.ValidateCollection(

            () => ValidateIfGuidIsNotEpmty(command.PetId, "Pet id"),

            () => ValidateIfGuidIsNotEpmty(command.VolunteerId, "Volunteer id"),

            () => ValidateRequiredField(
                command.PetName, "Pet name", MAX_LENGTH_SHORT_TEXT, NAME_PATTERN),

            () => ValidateNonRequiredField(
                command.Description, "Pet description", MAX_LENGTH_LONG_TEXT),

            () => ValidateNumber(
                command.Weight, "Pet weight", 0, 500),

            () => ValidateNumber(
                command.Height, "Pet height", 0, 500),

            () => ValidateNonRequiredField(
                command.Color, "Pet color", MAX_LENGTH_SHORT_TEXT, NAME_PATTERN),

            () => Phone.ValidateNonRequired(
                command.PhoneNumber, command.PhoneRegion),

            () => ValidateNonRequiredField(
                command.HealthInfo, "Pet info about health", MAX_LENGTH_LONG_TEXT),

            () => ValidateNumber(
                command.HelpStatus, "Pet HelpStatus", 0, Enum.GetValues<HelpStatus>().Length),

            () => Address.ValidateNonRequired(
                command.Region, command.City, command.Street, command.HomeNumber),

            () => ValidateItems(
                command.Requisites, r => RequisitesInfo.Validate(r.Name, r.Description)),

            () => PetType.Validate(
                BreedID.SetValue(command.BreedId), SpeciesID.SetValue(command.SpeciesId)));
    }

}
