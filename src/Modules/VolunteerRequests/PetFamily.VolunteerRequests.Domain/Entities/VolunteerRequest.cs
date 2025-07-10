using PetFamily.SharedKernel.Abstractions;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.VolunteerRequests.Domain.Enums;
using static PetFamily.SharedKernel.Validations.ValidationConstants;
using static PetFamily.SharedKernel.Validations.ValidationExtensions;
using static PetFamily.SharedKernel.Validations.ValidationPatterns;

namespace PetFamily.VolunteerRequests.Domain.Entities;

public class VolunteerRequest : SoftDeletable, IEntity<Guid>
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid? AdminId { get; private set; }
    public Guid? DiscussionId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? RejectedAt { get; private set; }
    public RequestStatus RequestStatus { get; private set; }
    public string? RejectedComment { get; private set; }
    public string DocumentName { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Description { get; private set; }
    public int ExperienceYears { get; private set; }

    public IEnumerable<RequisitesInfo> Requisites { get; private set; }

    private VolunteerRequest() { }//EFCore need this

    private VolunteerRequest(
        Guid id,
        Guid userId,
        string documentName,
        string lastName,
        string firstName,
        string description,
        int experienceYears,
        IEnumerable<RequisitesInfo> requisites
        )
    {
        Id = id;
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
        RequestStatus = RequestStatus.Created;
        DocumentName = documentName;
        LastName = lastName;
        FirstName = firstName;
        Description = description;
        ExperienceYears = experienceYears;
        Requisites = requisites;
    }

    public static Result<VolunteerRequest> Create(
        Guid userId,
        string documentName,
        string lastName,
        string firstName,
        string description,
        int experienceYears,
        IEnumerable<RequisitesInfo> requisites)
    {
        var validationResult = Validate(
            userId,
            documentName,
            lastName,
            firstName,
            description,
            experienceYears);

        if (validationResult.IsFailure)
            return validationResult;

        var request = new VolunteerRequest(
            Guid.NewGuid(),
            userId,
            documentName,
            lastName,
            firstName,
            description,
            experienceYears,
            requisites);

        return Result.Ok(request);
    }

    public static UnitResult Validate(
        Guid userId,
        string documentName,
        string lastName,
        string firstName,
        string description,
        int experienceYears)
    {
        return UnitResult.FromValidationResults(
            () => ValidateIfGuidIsNotEpmty(userId, "UserId"),
            () => ValidateRequiredField(documentName, "documentName", MAX_LENGTH_MEDIUM_TEXT),
            () => ValidateRequiredField(firstName, "FirstName", MAX_LENGTH_SHORT_TEXT, NAME_PATTERN),
            () => ValidateRequiredField(lastName, "LastName", MAX_LENGTH_SHORT_TEXT, NAME_PATTERN),
            () => ValidateNonRequiredField(description, "description", MAX_LENGTH_LONG_TEXT),
            () => ValidateIntegerNumber(experienceYears, "ExperienceYears", 0, 100));
    }

    public UnitResult TakeToReview(Guid adminId, Guid discussionId)
    {
        if(RequestStatus != RequestStatus.Created)
            return UnitResult.Fail(Error.InternalServerError("Volunteer request is already taken for review."));

        AdminId = adminId;
        DiscussionId = discussionId;
        RequestStatus = RequestStatus.OnReview;
        UpdatedAt = DateTime.UtcNow;
        return UnitResult.Ok();
    }

    public UnitResult Approve(Guid adminId)
    {
        if (RequestStatus == RequestStatus.Approved)
            return UnitResult.Fail(Error.Conflict("Request is already approved."));
        
        if (AdminId != adminId)
            return UnitResult.Fail(Error.Conflict("Only the admin who took the request can approve it."));

        RequestStatus = RequestStatus.Approved;
        UpdatedAt = DateTime.UtcNow;
        RejectedAt = null;
        return UnitResult.Ok();
    }

    public UnitResult Reject(Guid adminId,string comment)
    {
        if(AdminId != adminId)
            return UnitResult.Fail(Error.Conflict("Only the admin who took the request can reject it."));

        if (RequestStatus != RequestStatus.OnReview)
            return UnitResult.Fail(Error.Conflict("Request cannot be rejected because it's status must be 'onReview!' "));

        RejectedComment = comment;
        RequestStatus = RequestStatus.Rejected;
        UpdatedAt = DateTime.UtcNow;
        RejectedAt = UpdatedAt;

        return UnitResult.Ok();
    }
    public UnitResult SendBackToRevision(Guid adminId)
    {
        if (RequestStatus != RequestStatus.OnReview)
            return UnitResult.Fail(Error.Conflict("Only requests under review can be sent back to revision."));

        RequestStatus = RequestStatus.NeedsRevision;
        UpdatedAt = DateTime.UtcNow;
        return UnitResult.Ok();
    }

    public UnitResult CancelSendBackToRevision(Guid adminId)
    {
        if (RequestStatus != RequestStatus.NeedsRevision)
            return UnitResult.Fail(Error.Conflict("Only requests that were sent back to revision can be cancelled."));

        RequestStatus = RequestStatus.OnReview;
        UpdatedAt = DateTime.UtcNow;
        return UnitResult.Ok();
    }

    public UnitResult TryRestoreAfterRejectionExpiration(int rejectionDays)
    {
        if (RequestStatus != RequestStatus.Rejected || !RejectedAt.HasValue)
            return UnitResult.Fail(Error.Conflict("Request is not in a rejected state."));

        if (RejectedAt.Value.AddDays(rejectionDays) <= DateTime.UtcNow)
        {
            RejectedAt = null; 
            RequestStatus = RequestStatus.NeedsRevision;
            UpdatedAt = DateTime.UtcNow;
            return UnitResult.Ok();
        }

        return UnitResult.Fail(Error.Conflict("Rejection period has not expired yet."));
    }

    public Result<TimeSpan> GetRejectionRemainingTime(int rejectionDays)
    {
        if (RequestStatus != RequestStatus.Rejected)
            return Result.Fail(Error.Conflict("Only rejected requests can be checked for rejection date."));

        if (!RejectedAt.HasValue)
            return Result.Fail(Error.StringIsNullOrEmpty("Rejection date"));

        var expirationDate = RejectedAt.Value.AddDays(rejectionDays);
        var now = DateTime.UtcNow;

        if (now >= expirationDate)
            return Result.Fail(Error.Conflict("Rejection period has already expired."));

        var remaining = expirationDate - now;
        return Result.Ok(remaining);
    }

    public UnitResult CancelApproval(Guid adminId)
    {
        if (RequestStatus != RequestStatus.Approved)
            return UnitResult.Fail(Error.Conflict("Only approved requests can be cancelled."));

        if (AdminId != adminId)
            return UnitResult.Fail(Error.Conflict("Only the admin who approved the request can cancel it."));

        RequestStatus = RequestStatus.Created;
        UpdatedAt = DateTime.UtcNow;

        return UnitResult.Ok();
    }

    public UnitResult UpdateRequestDetails(
        string firstName,
        string lastName, 
        int experienceYears,
        string documentName,
        string description)
    {
        if (RequestStatus != RequestStatus.NeedsRevision)
            return Result.Fail(Error.Conflict("Only requests marked for revision can be updated"));

        Description = description;
        FirstName = firstName;
        LastName = lastName;
        ExperienceYears = experienceYears;
        DocumentName = documentName;
        UpdatedAt = DateTime.UtcNow;
        RequestStatus = RequestStatus.OnReview;

        return UnitResult.Ok();
    }
}

