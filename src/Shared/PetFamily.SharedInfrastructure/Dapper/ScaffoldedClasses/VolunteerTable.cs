using PetFamily.SharedInfrastructure.Constants;
namespace PetFamily.SharedInfrastructure.Shared.Dapper.ScaffoldedClasses;

public static class VolunteerTable
{
    public const string TableName = "volunteers";
    public const string TableFullName = SchemaNames.VOLUNTEER + "." + TableName;
    public const string Id = "id";
    public const string Email = "email";
    public const string ExperienceYears = "experience_years";
    public const string Description = "description";
    public const string Rating = "rating";
    public const string Requisites = "requisites";
    public const string SocialNetworks = "social_networks";
    public const string DeletedDateTime = "deleted_date_time";
    public const string IsDeleted = "is_deleted";
    public const string FirstName = "first_name";
    public const string LastName = "last_name";
    public const string PhoneNumber = "phone_number";
    public const string PhoneRegionCode = "phone_region_code";
}
