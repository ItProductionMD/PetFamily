
using PetFamily.Application.Abstractions;

namespace PetFamily.Application.Commands.SharedCommands;

public record RestoreVolunteerCommand(Guid VolunteerId) : ICommand;

