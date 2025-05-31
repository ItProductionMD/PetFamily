using Volunteers.Application.Commands.PetManagement.ChangePetsOrder;

namespace Volunteers.Presentation.Requests;

public record ChangePetsOrderRequest(List<Guid> PetsOrder)
{
    public ChangePetsOrderCommand ToCommand(Guid volunteerId)
    {
        return new(volunteerId, PetsOrder);
    }
}

