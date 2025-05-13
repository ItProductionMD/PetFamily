using PetFamily.Application.Commands.PetManagment.ChangePetsOrder;

namespace PetFamily.API.Requests;

public record ChangePetsOrderRequest(List<Guid> PetsOrder)
{
    public ChangePetsOrderCommand ToCommand(Guid volunteerId)
    {
        return new(volunteerId, PetsOrder);
    }
}

