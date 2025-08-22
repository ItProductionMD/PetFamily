using Moq;
using PetFamily.Discussions.Application.Dtos;
using PetFamily.Discussions.Application.Queries.GetDiscussion;
using PetFamily.Discussions.Domain.Entities;
using PetFamily.Discussions.Infrastructure.Contexts;
using PetFamily.Discussions.Public.Dtos;
using PetFamily.IntegrationTests.IClassFixtures;
using PetFamily.IntegrationTests.WebApplicationFactory;
using PetFamily.SharedKernel.Results;

namespace PetFamily.IntegrationTests.DiscussionFeatures;

public class GetDiscussionQueryTest : QueryHandlerTest<GetDiscussionResponse, GetDiscussionQuery>
{
    private DiscussionTestDataCreator discussionCreator ;
    private Guid adminId;
    private Guid userId;
    private Guid discussionId;

    public GetDiscussionQueryTest(TestWebApplicationFactory factory) : base(factory)
    {
        discussionCreator = new DiscussionTestDataCreator();
        adminId = discussionCreator.AdminId;
        userId = discussionCreator.UserId;
    }

    [Fact]
    public async Task Should_GetDiscussion_Successfully()
    {
        //ARRANGE
        var discussion = discussionCreator.CreateDiscussion();

        await SeedAsync(typeof(DiscussionWriteDbContext), discussion);

        var command = new GetDiscussionQuery(adminId, discussion.Id, 1, 10);

        var participants = new List<ParticipantDto>
        {
            new ParticipantDto(adminId, "Admin FullName", true),
            new ParticipantDto(userId, "User FullName", false)
        };

        var resultParticipants = Result<List<ParticipantDto>>.Ok(participants);

        _factory.ParticipantContractMock.Setup(p =>
            p.GetByIds(It.IsAny<IReadOnlyList<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(resultParticipants);

        //ACT
        var result = await _sut.Handle(command, CancellationToken.None);
        //ASSERT
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(discussion.Id, result.Data.DiscussionDto.Id);
    }
}
