using System.Security.Cryptography.X509Certificates;

namespace PetFamily.Discussions.Application.Dtos;

public record DiscussionDto(
    Guid Id,
    bool IsClosed,
    IReadOnlyList<Guid> ParticipantIds,
    List<DiscussionMessageDto> Messages
);
