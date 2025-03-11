using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.PetManagment.ValueObjects;
using PetFamily.Domain.PetManagment.Enums;
using PetFamily.Domain.PetManagment.Root;

namespace TestPetFamilyDomain;

public static class TestDataFactory
{
    public static Phone ValidPhoneNumber = Phone.CreateNotEmpty("695556621", "+39").Data!;
    public static Volunteer CreateVolunteer(int numberOfPets)
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

        SetUpVolunteer(volunteer, numberOfPets);

        return volunteer!;
    }

    public static readonly List<string> FIRST_NAMES =
    [
        "John", "Jane", "Alice", "Bob", "Charlie", "David", "Eve", "Frank",
        "Grace", "Heidi", "Ivan", "Jack", "Kate", "Liam", "Mia", "Nina",
        "Oliver", "Pam", "Quinn", "Ruth", "Steve", "Tina", "Ursula", "Vlad",
        "Wendy", "Xander", "Yvonne", "Zack"
    ];

    public static readonly List<string> LAST_NAMES =
    [
        "Smith", "Johnson", "Williams", "Jones", "Brown", "Davis", "Miller",
        "Wilson", "Moore", "Taylor", "Anderson", "Thomas", "Jackson", "White",
        "Harris", "Martin", "Thompson", "Garcia", "Martinez", "Robinson",
        "Clark", "Rodriguez", "Lewis", "Lee", "Walker", "Hall", "Allen",
        "Young", "Hernandez"
    ];

    public static readonly List<string> NICKNAMES =
    [
        "Rex", "Tom", "Jerry", "Max", "Bella", "Charlie", "Lucy", "Daisy",
        "Bailey", "Buddy", "Molly", "Lola", "Sadie", "Sophie", "Maggie",
        "Chloe", "Duke", "Rocky", "Luna", "Harley", "Milo", "Bear", "Toby",
        "Jack", "Oscar", "Teddy", "Cooper", "Riley", "Zoe"
    ];
    private static void SetUpVolunteer(Volunteer volunteer, int numberOfPets)
    {
        for (int i = 0; i < numberOfPets; i++)
        {
            volunteer.CreateAndAddPet(
                NICKNAMES[0],
                new DateOnly(2019, 1, 1),
                "",
                true,
                false,
                1.00,
                1.00,
                "",
                PetType.Create(BreedID.NewGuid(), SpeciesID.NewGuid()).Data!,
                Phone.CreateNotEmpty("+373", "123456789").Data!,
                [RequisitesInfo.Create("Bank1", "Nr. 765753757835157").Data!],
                HelpStatus.ForHelp,
                "",
                Address.CreatePossibleEmpty("", "", "", "11").Data!);
        }
    }

}

