using PetFamily.Application.Abstractions.CQRS;

namespace Volunteers.Application.Commands.PetManagement.ChangePetsOrder;

public record ChangePetsOrderCommand(Guid VolunteerId, List<Guid> petsNewOrder) : ICommand;


