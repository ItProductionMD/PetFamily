using PetFamily.Application.Abstractions;
using PetFamily.Domain.Shared.ValueObjects;

namespace PetFamily.Application.Commands.SharedCommands;

public record VolunteerDtoCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string Description,
    string PhoneRegionCode,
    string PhoneNumber,
    int ExperienceYears,
    int PetsCount) : ICommand;
