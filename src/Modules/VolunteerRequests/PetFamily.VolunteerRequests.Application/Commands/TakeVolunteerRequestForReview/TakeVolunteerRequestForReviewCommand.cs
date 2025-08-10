using PetFamily.Application.Abstractions.CQRS;

namespace PetFamily.VolunteerRequests.Application.Commands.TakeVolunteerRequestForReview;

public record TakeVolunteerRequestForReviewCommand(Guid VolunteerRequestId) : ICommand;

