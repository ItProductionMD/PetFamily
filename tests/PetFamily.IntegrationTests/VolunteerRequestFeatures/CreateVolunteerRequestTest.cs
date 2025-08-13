using Microsoft.EntityFrameworkCore;
using Moq;
using PetFamily.IntegrationTests.IClassFixtures;
using PetFamily.IntegrationTests.WebApplicationFactory;
using PetFamily.SharedApplication.Exceptions;
using PetFamily.SharedApplication.IUserContext;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.VolunteerRequests.Application.Commands.SubmitVolunteerRequest;
using PetFamily.VolunteerRequests.Domain.Entities;

namespace PetFamily.IntegrationTests.VolunteerRequestFeatures;

public class CreateVolunteerRequestTest(TestWebApplicationFactory factory)
    : CommandHandlerTest<SubmitVolunteerRequestCommand>(factory)
{
    [Fact]
    public async Task Should_create_one_volunteer_request_successfully()
    {
        //ARRANGE
        var userId = Guid.NewGuid();

        var command = new SubmitVolunteerRequestCommand(
            userId,
            "file.doc",
            "Iurii",
            "Godina",
            "description",
            1,
            [new("victoriabank", "iban12345")]);

        //ACT
        var handleResult = await _sut.Handle(command, CancellationToken.None);
        //ASSERT
        Assert.True(handleResult.IsSuccess);

        var addedVolunteerRequest = await _volunteerRequestDbContext.VolunteerRequests
            .AsNoTracking()
            .FirstOrDefaultAsync();

        AssertCustom.AreEqualData(command, addedVolunteerRequest);
    }

    [Fact]
    public async Task Should_fail_if_request_already_exists()
    {
        var userId = Guid.NewGuid();

        // ARRANGE
        var command = new SubmitVolunteerRequestCommand(
            userId,
            "file.doc",
            "Iurii",
            "Godina",
            "description",
            1,
            [new("victoriabank", "iban12345")]);

        var existingRequest = VolunteerRequest.Create(
            userId,
            command.DocumentName,
            command.LastName,
            command.FirstName,
            command.Description,
            command.ExperienceYears,
            command.Requisites.Select(r => RequisitesInfo.Create(r.Name, r.Description).Data!).ToList()
        ).Data!;

        await _volunteerRequestDbContext.VolunteerRequests.AddAsync(existingRequest);
        await _volunteerRequestDbContext.SaveChangesAsync();

        // ACT
        var result = await _sut.Handle(command, CancellationToken.None);

        // ASSERT
        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Conflict, result.Error.Type);
    }

    [Fact]
    public async Task Should_fail_if_first_name_is_empty()
    {
        // ARRANGE
        var userId = Guid.NewGuid();

        var command = new SubmitVolunteerRequestCommand(
            userId,
            "file.doc",
            "Iurii",
            "",
            "description",
            1,
            [new("victoriabank", "iban12345")]);

        // ACT + ASSERT
        await Assert.ThrowsAsync<ValidationException>(() => _sut.Handle(command, CancellationToken.None));
    }

}
