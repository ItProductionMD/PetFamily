using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Commands.VolunteerManagment.CreateVolunteer;
using PetFamily.Application.Queries.Volunteer.GetVolunteers;
using PetFamily.Infrastructure.Contexts;
using System;
using PetFamily.Application.Abstractions;
using System.ComponentModel.DataAnnotations;
using PetFamily.Domain.DomainError;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.IntegrationTests.TestData;
using PetFamily.IntegrationTests.Seeds;
using PetFamily.Domain.PetManagment.Root;

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

        var addedVolunteer = await _dbContext.Volunteers
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

        await Seeder.Seed(seedVolunteer,_dbContext);

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
