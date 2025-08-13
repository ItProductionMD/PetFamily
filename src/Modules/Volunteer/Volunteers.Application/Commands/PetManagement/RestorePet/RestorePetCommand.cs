using PetFamily.SharedApplication.Abstractions.CQRS;

namespace Volunteers.Application.Commands.PetManagement.RestorePet;

public record RestorePetCommand(Guid VolunteerId, Guid PetId) : ICommand;
