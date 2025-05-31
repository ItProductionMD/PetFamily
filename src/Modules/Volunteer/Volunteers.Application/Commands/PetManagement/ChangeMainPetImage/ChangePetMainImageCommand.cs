using PetFamily.Application.Abstractions.CQRS;

namespace Volunteers.Application.Commands.PetManagement.ChangeMainPetImage;

public record ChangePetMainImageCommand(Guid VolunteerId, Guid PetId, string imageName) : ICommand;

