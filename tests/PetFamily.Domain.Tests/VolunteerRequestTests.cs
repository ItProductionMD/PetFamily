using System;
using System.Collections.Generic;
using FluentAssertions;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.Auth.Domain.Entities.VolunteerRequestAggregate;
using PetFamily.Auth.Domain.Enums;
using Xunit;

namespace TestPetFamilyDomain;

public class VolunteerRequestTests
{
    [Fact]
    public void Create_ShouldReturnSuccess_WhenDataIsValid()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var requisites = new List<RequisitesInfo>();

        // Act
        var result = VolunteerRequest.Create(
            id,
            userId,
            "document.pdf",
            "Doe",
            "John",
            "I love animals",
            5,
            requisites);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.UserId.Should().Be(userId);
        result.Data!.RequestStatus.Should().Be(RequestStatus.Created);
    }

    [Theory]
    [InlineData("", "Doe", "John")]
    [InlineData("document.pdf", "", "John")]
    [InlineData("document.pdf", "Doe", "")]
    public void Create_ShouldReturnFailure_WhenRequiredFieldsInvalid(string doc, string last, string first)
    {
        // Act
        var result = VolunteerRequest.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            doc,
            last,
            first,
            "desc",
            2,
            new List<RequisitesInfo>());

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void TakeToReview_ShouldUpdateStatusAndAdminInfo()
    {
        var request = CreateValidRequest();
        var adminId = Guid.NewGuid();
        var discussionId = Guid.NewGuid();

        request.TakeToReview(adminId, discussionId);

        request.RequestStatus.Should().Be(RequestStatus.OnReview);
        request.AdminId.Should().Be(adminId);
        request.DiscussionId.Should().Be(discussionId);
    }

    [Fact]
    public void Approve_ShouldSetStatusToApproved()
    {
        var request = CreateValidRequest();
        var adminId = Guid.NewGuid();

        request.Approve(adminId);

        request.RequestStatus.Should().Be(RequestStatus.Approved);
        request.AdminId.Should().Be(adminId);
    }

    [Fact]
    public void Reject_ShouldSetStatusToRejectedAndStoreComment()
    {
        var request = CreateValidRequest();
        var adminId = Guid.NewGuid();
        var comment = "Not enough experience.";

        request.Reject(adminId, comment);

        request.RequestStatus.Should().Be(RequestStatus.Rejected);
        request.AdminId.Should().Be(adminId);
        request.RejectedComment.Should().Be(comment);
    }

    private static VolunteerRequest CreateValidRequest()
    {
        return VolunteerRequest.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "doc.pdf",
            "Doe",
            "John",
            "Some description",
            3,
            new List<RequisitesInfo>()).Data!;
    }
}

