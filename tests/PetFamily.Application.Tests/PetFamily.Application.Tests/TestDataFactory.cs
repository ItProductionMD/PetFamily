using PetFamily.Domain.PetManagment.Root;
using PetFamily.Domain.PetManagment.ValueObjects;
using PetFamily.Domain.Shared.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetFamily.Application.Tests;

public static class TestDataFactory
{
    public static Volunteer CreateVolunteer()
    {
        //
        var fullName = FullName.Create("John", "Doe").Data!;
        var phoneNumber = Phone.CreateNotEmpty("123456789", "+373").Data!;
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
            fullName,
            email,
            phoneNumber,
            0,
            null,
            donateDetailsList,
            socialNetworksList).Data!;

        return volunteer!;
    }
}
