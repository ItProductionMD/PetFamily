namespace PetFamily.Discussions.Application.Dtos;

public record DiscussionMessageDto(
    Guid Id,
    Guid AuthorId,
    string Text,
    DateTime CreatedAt
);

