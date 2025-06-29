using PetFamily.Auth.Domain.Enums;
using PetFamily.SharedKernel.Abstractions;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects;
using static PetFamily.SharedKernel.Validations.ValidationConstants;
using static PetFamily.SharedKernel.Validations.ValidationExtensions;
using static PetFamily.SharedKernel.Validations.ValidationPatterns;

namespace PetFamily.Auth.Domain.Entities.VolunteerRequestAggregate;

public class VolunteerRequest : SoftDeletable, IEntity<Guid>
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid? AdminId { get; private set; }
    public Guid? DiscussionId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
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
        Guid id,
        Guid userId,
        string documentName,
        string lastName,
        string firstName,
        string description,
        int experienceYears,
        IEnumerable<RequisitesInfo> requisites)
    {
        var validationResult = Validate(
            id,
            userId,
            documentName,
            lastName,
            firstName,
            description,
            experienceYears,
            requisites);

        if (validationResult.IsFailure)
            return validationResult;

        var request = new VolunteerRequest(
            id,
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
        Guid id,
        Guid userId,
        string documentName,
        string lastName,
        string firstName,
        string description,
        int experienceYears,
        IEnumerable<RequisitesInfo> requisites)
    {
        return UnitResult.FromValidationResults(
            () => ValidateIfGuidIsNotEpmty(id, "VolunteerRequest"),
            () => ValidateIfGuidIsNotEpmty(userId, "UserId"),
            () => ValidateRequiredField(documentName, "documentName", MAX_LENGTH_MEDIUM_TEXT),
            () => ValidateRequiredField(firstName, "FirstName", MAX_LENGTH_SHORT_TEXT, NAME_PATTERN),
            () => ValidateRequiredField(lastName, "LastName", MAX_LENGTH_SHORT_TEXT, NAME_PATTERN),
            () => ValidateNonRequiredField(description, "description", MAX_LENGTH_LONG_TEXT),
            () => ValidateIntegerNumber(experienceYears, "ExperienceYears", 0, 100));
    }

    public void TakeToReview(Guid adminId, Guid discussionId)
    {
        AdminId = adminId;
        DiscussionId = discussionId;
        RequestStatus = RequestStatus.OnReview;
    }

    public void Approve(Guid adminId)
    {
        AdminId = adminId;
        RequestStatus = RequestStatus.Approved;
    }

    public void Reject(Guid adminId, string comment)
    {
        AdminId = adminId;
        RejectedComment = comment;
        RequestStatus = RequestStatus.Rejected;
    }
}
