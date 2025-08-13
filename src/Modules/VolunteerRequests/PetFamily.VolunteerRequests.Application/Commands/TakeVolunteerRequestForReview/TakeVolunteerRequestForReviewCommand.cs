using PetFamily.SharedApplication.Abstractions.CQRS;

namespace PetFamily.VolunteerRequests.Application.Commands.TakeVolunteerRequestForReview;

public record TakeVolunteerRequestForReviewCommand(Guid AdminId, Guid VolunteerRequestId) : ICommand;

