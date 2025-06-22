using PetFamily.SharedKernel.ValueObjects;
using PetFamily.SharedKernel.ValueObjects.Ids;
using Volunteers.Domain;
using Volunteers.Domain.ValueObjects;

namespace PetFamily.Application.Tests;

public static class TestDataFactory
{
    public static Volunteer CreateVolunteer()
    {
        //
        var fullName = FullName.Create("John", "Doe").Data!;
        var phone = Phone.CreateNotEmpty("123456789", "+373").Data!;
        var email = "testemail@m.com";

        var volunteerId = VolunteerID.NewGuid();

        var donateDetailsList = new List<RequisitesInfo>
        {
            RequisitesInfo.Create("Bank1", "Nr. 765753757835157").Data!,
            RequisitesInfo.Create("Bank2", "Nr. 765753758345157").Data!
        };

        var socialNetworksList = new List<SocialNetworkInfo>
        {
            SocialNetworkInfo.Create("Facebook", "https://www.facebook.com").Data!,
            SocialNetworkInfo.Create("Google", "https://www.google.com").Data!
        };

        var volunteer = Volunteer.Create(
            volunteerId,
            UserId.NewGuid(),
            fullName,
            0,
            null,
            phone,
            donateDetailsList).Data!;

        return volunteer!;
    }
}
