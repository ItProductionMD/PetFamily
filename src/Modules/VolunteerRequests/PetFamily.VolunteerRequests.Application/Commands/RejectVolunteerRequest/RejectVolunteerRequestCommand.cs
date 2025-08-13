using PetFamily.SharedApplication.Abstractions.CQRS;
namespace PetFamily.VolunteerRequests.Application.Commands.RejectVolunteerRequest;

public record RejectVolunteerRequestCommand(Guid AdminId, Guid VolunteerRequestId, string Comment) : ICommand;

