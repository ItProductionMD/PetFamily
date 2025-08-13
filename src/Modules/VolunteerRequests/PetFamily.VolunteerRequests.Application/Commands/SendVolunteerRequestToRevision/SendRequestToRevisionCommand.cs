using PetFamily.SharedApplication.Abstractions.CQRS;

namespace PetFamily.VolunteerRequests.Application.Commands.SendVolunteerRequestToRevision;

public record SendRequestToRevisionCommand(Guid AdminId, Guid VolunteerRequestId, string Comment) : ICommand;

