using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PetFamily.Domain.Shared.Validations.ValidationExtensions;
using static PetFamily.Domain.Shared.Validations.ValidationConstants;
using static PetFamily.Domain.Shared.Validations.ValidationPatterns;
using static PetFamily.Application.Volunteers.SharedVolunteerRequests;
using static PetFamily.Application.Validations.ValidationExtensions;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.PetAggregates.Enums;
using PetFamily.Application.SharedValidations;
using PetFamily.Domain.PetAggregates.ValueObjects;
using PetFamily.Domain.Results;

namespace PetFamily.Application.Volunteers.AddPet;

public static class AddPetCommandValidator
{
    public static UnitResult Validate(AddPetCommand command)
    {
        return UnitResult.ValidateCollection(

            () => ValidateRequiredField(command.PetName, "Pet name", MAX_LENGTH_SHORT_TEXT),

            () => ValidateNonRequiredField(
                command.Description, "Pet description", MAX_LENGTH_LONG_TEXT),

            () => ValidateNumber(command.Weight, "Pet weight", 0, 500),

            () => ValidateNumber(command.Height, "Pet height", 0, 500),

            () => ValidateNonRequiredField(command.Color, "Pet color", MAX_LENGTH_SHORT_TEXT),

            () => Phone.Validate(command.OwnerPhoneNumber, command.OwnerPhoneRegion),

            () => ValidateNonRequiredField(
                command.HealthInfo, "Pet info about health", MAX_LENGTH_LONG_TEXT),

            () => ValidateNumber(
                command.HelpStatus, "Pet HelpStatus", 0, Enum.GetValues(typeof(HelpStatus)).Length),

            () => Address.Validate(
                command.Region, command.City, command.Street, command.HomeNumber),

            () => ValidateItems(
                command.DonateDetails, r => RequisitesInfo.Validate(r.Name, r.Description)),

            () => PetType.Validate(
                BreedID.SetValue(command.BreedId), SpeciesID.SetValue(Guid.NewGuid())));
    }
}
