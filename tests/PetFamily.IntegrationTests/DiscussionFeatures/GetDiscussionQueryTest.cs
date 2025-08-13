using Microsoft.Extensions.DependencyInjection;
using Moq;
using PetFamily.Auth.Public.Contracts;
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
    public GetDiscussionQueryTest(TestWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_GetDiscussion_Successfully()
    {
        //ARRANGE
        var requestId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var adminId = Guid.NewGuid();


        var discussion = Discussion.Create(requestId,adminId,userId).Data!;

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
