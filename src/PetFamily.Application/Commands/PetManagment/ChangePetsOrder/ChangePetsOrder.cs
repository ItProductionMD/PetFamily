
using Microsoft.Extensions.Logging;
using PetFamily.Application.IRepositories;
using PetFamily.Domain.DomainError;
using PetFamily.Domain.PetManagment.ValueObjects;
using PetFamily.Domain.Results;

namespace PetFamily.Application.Commands.PetManagment.ChangePetsOrder;

public class ChangePetsOrder(
    IVolunteerRepository volunteerRepository,
    ILogger<ChangePetsOrder> logger)
{
    private readonly IVolunteerRepository _volunteerRepository = volunteerRepository;
    private readonly ILogger<ChangePetsOrder> _logger = logger;

    public async Task<UnitResult> Handle(
        ChangePetsOrderCommand command,
        CancellationToken cancellationToken)
    {
        if (command.petsNewOrder.Count == 0)
            return Result.Fail(Error.InvalidLength("Pets new order"));

        var volunteer = await _volunteerRepository.GetByIdAsync(command.VolunteerId, cancellationToken);

        var changePetsOrderResult = volunteer.ChangePetsOrder(command.petsNewOrder);

        if (changePetsOrderResult.IsFailure)
        {
            _logger.LogError("Change pets order error for volunteer with Id:{id}!Errors:{errors}",
                volunteer.Id, changePetsOrderResult.ToErrorMessages());
            return changePetsOrderResult;
        }
        await _volunteerRepository.Save(volunteer, cancellationToken);

        _logger.LogInformation("Change pets order for volunteer id:{Id} succesfull!", volunteer.Id);

        return UnitResult.Ok();
    }
}


