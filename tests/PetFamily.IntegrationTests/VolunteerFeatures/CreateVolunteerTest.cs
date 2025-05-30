﻿using Microsoft.EntityFrameworkCore;
using PetFamily.IntegrationTests.Seeds;
using PetFamily.IntegrationTests.TestData;
using Volunteers.Application.Commands.VolunteerManagement.CreateVolunteer;

namespace PetFamily.IntegrationTests.VolunteerFeatures;

public class CreateVolunteerTest(TestWebApplicationFactory factory)
    : CommandHandlerTest<Guid, CreateVolunteerCommand>(factory)
{
    [Fact]
    public async Task Should_create_one_volunteer_successfully()
    {
        //ARRANGE
        var command = new CreateVolunteerCommand(
            "Iurii",
            "Godina",
            "iyrii@gmail.com",
            "description",
            "68756643",
            "+373",
            1,
            [new("victoriabank", "iban12345")],
            [new("google", "google.com/aswmsnjnij/jneijn")]);

        //ACT
        var handleResult = await _sut.Handle(command, CancellationToken.None);

        //ASSERT
        Assert.True(handleResult.IsSuccess);

        var addedVolunteer = await _volunteerDbContext.Volunteers
            .AsNoTracking()
            .FirstOrDefaultAsync();

        AssertCustom.AreEqualData(command, addedVolunteer);
    }

    [Theory]
    [InlineData(false, true)]//email error and phone ok
    [InlineData(true, false)]//email ok and phone error
    [InlineData(false, false)]//email and phone error

    public async Task Should_create_one_volunteer_with_uniqueness_error(
        bool isEmailUniquenessOK,
        bool isPhoneUniquenessOk)
    {
        //ARRANGE
        var seedVolunteer = new VolunteerTestBuilder().Volunteer;

        await DbContextSeedExtensions.SeedAsync(_volunteerDbContext, seedVolunteer);

        var command = new CreateVolunteerCommand(
            "Iurii",
            "Godina",
            isEmailUniquenessOK ? "uniqueEmail@gmail.com" : seedVolunteer.Email,
            "description",
            isPhoneUniquenessOk ? "12312310" : seedVolunteer.Phone.Number,
            seedVolunteer.Phone.RegionCode,
            1,
            [new("victoriabank", "iban12345")],
            [new("google", "google.com/aswmsnjnij/jneijn")]);

        //ACT
        var handleResult = await _sut.Handle(command, CancellationToken.None);

        //ASSERT
        Assert.True(handleResult.IsFailure);

        if (isEmailUniquenessOK == false)
            Assert.Contains(handleResult.Error.ValidationErrors, e => e.ObjectName == "Email");

        if (isPhoneUniquenessOk == false)
            Assert.Contains(handleResult.Error.ValidationErrors, e => e.ObjectName == "Phone");
    }
}
