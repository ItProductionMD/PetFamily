using System.Security.Cryptography.X509Certificates;

namespace PetFamily.Discussions.Application.Dtos;

public class DiscussionDto
{
    public Guid Id { get; set; }
    public bool IsClosed { get; set; }
    public IReadOnlyList<Guid> ParticipantIds { get; set; } = new List<Guid>();
    public List<DiscussionMessageDto> Messages { get; set; } = new List<DiscussionMessageDto>();
}