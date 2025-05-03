using Bogus.DataSets;
using PetFamily.Domain.PetManagment.Root;
using PetFamily.Domain.PetManagment.ValueObjects;
using PetFamily.Domain.PetTypeManagment.Root;
using PetFamily.Domain.Results;
using PetFamily.Domain.Shared.ValueObjects;

namespace PetFamily.IntegrationTests.TestData;

public class VolunteerTestBuilder
{
    public Volunteer Volunteer => Volunteers.First();
    public List<Volunteer> Volunteers { get; set; } = [];
    public const string VALID_EMAIL = "email@gmail.com";
    public const int VALID_PHONE_NUMBER = 6111111;
    public const string VALID_PHONE_CODE = "+373";

    public VolunteerTestBuilder(string? email = null,int? phoneNumber = VALID_PHONE_NUMBER, int? volunteersCount = 1)
    {
        email ??= VALID_EMAIL;
        for (int i = 0; i < volunteersCount; i++)
        {
            var newPhoneNumber = (i + phoneNumber).ToString();

            var phoneResult = Phone.CreateNotEmpty(newPhoneNumber, VALID_PHONE_CODE);
            if (phoneResult.IsFailure)
                throw new Exception($"Cant create volunteer with builder!Error:" +
                    $"{phoneResult.ValidationMessagesToString()}");

            var result = Volunteer.Create(
                VolunteerID.NewGuid(),
                FullName.Create("FirstName", "LastName").Data!,
                i + email,
                phoneResult.Data!,
                 1,
                "test description",
                [],
                []
            );
            if (result.IsFailure)
                throw new Exception($"Cant create volunteer with builder!Error:" +
                    $"{result.ValidationMessagesToString()}");

            Volunteers.Add(result.Data!);
        }
    }

    public VolunteerTestBuilder WithPets(int petsCount, Species species)
    {
        foreach(var volunteer in Volunteers)
        {
            for (int i = 0; i < petsCount; i++)
            {
                volunteer.CreateAndAddPet(
                    "petName",
                    null,
                    "description",
                    true,
                    true,
                    1,
                    1,
                    "color",
                    PetType.Create(
                        BreedID.SetValue(species.Breeds[0].Id),
                        SpeciesID.SetValue(species.Id)).Data!,
                    Phone.CreateEmpty(),
                    [],
                    Domain.PetManagment.Enums.HelpStatus.Helped,
                    "health info",
                    Domain.Shared.ValueObjects.Address.CreateEmpty());
            }
        } 
        return this;
    }
}
