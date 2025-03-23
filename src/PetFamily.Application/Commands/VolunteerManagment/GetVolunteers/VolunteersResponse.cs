using PetFamily.Application.Commands.SharedCommands;

namespace PetFamily.Application.Commands.VolunteerManagment.GetVolunteers;

public record VolunteersResponse(int ItemCounts,IEnumerable<VolunteerDtoCommand> Dtos);

