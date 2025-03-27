
using PetFamily.Application.Abstractions;

namespace PetFamily.Application.Commands.PetManagment.ChangePetsOrder;

public record ChangePetsOrderCommand(Guid VolunteerId,List<Guid> petsNewOrder) :ICommand;


