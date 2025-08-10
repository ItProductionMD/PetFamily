
using PetFamily.SharedKernel.ValueObjects;

namespace PetFamily.VolunteerRequests.Public.Dtos;

public record SubmitVolunteerRequestDto(
    Guid userId,
    string documentName,
    string lastName,
    string firstName,
    string description,
    int experienceYears,
    IEnumerable<RequisitesInfo> requisites);

