using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.Discussions.Public.Dtos;

namespace PetFamily.Discussions.Application.Dtos;

public record GetDiscussionResponse(
    List<ParticipantDto> Participants,
    DiscussionDto DiscussionDto);
