using PetFamily.Application.Abstractions.CQRS;

namespace PetFamily.VolunteerRequests.Application.Commands.SendVolunteerRequestToRevision;

public record SendRequestToRevisionCommand(Guid VolunteerRequestId, string Comment) : ICommand;

