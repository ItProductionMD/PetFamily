using PetFamily.SharedKernel.ValueObjects;
using PetFamily.SharedKernel.ValueObjects.Ids;
using PetSpecies.Domain;
using Volunteers.Domain;
using Volunteers.Domain.Enums;
using Volunteers.Domain.ValueObjects;
using static PetFamily.IntegrationTests.TestData.RandomAddressGenerator;
using static PetFamily.IntegrationTests.TestData.RandomEnumGenerator;

namespace PetFamily.IntegrationTests.TestData;

public class VolunteerTestBuilder
{
    public Volunteer Volunteer => Volunteers.First();
    private List<Species> speciesList = [];
    public List<Volunteer> Volunteers { get; set; } = [];
    public const string VALID_EMAIL = "email@gmail.com";
    public const string VALID_PHONE_CODE = "+373";
    public string[] Colors = ["black", "red", "gray", "yellow", "brown", "green", "white"];
    public const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

    private VolunteerTestBuilder(List<Species> speciesList)
    {
        this.speciesList = speciesList;
    }

    public VolunteerTestBuilder(string? email = null, int? phoneNumber = null, int? volunteersCount = 1)
    {
        email ??= VALID_EMAIL;
        for (int i = 0; i < volunteersCount; i++)
        {
            var newPhoneNumber = phoneNumber == null
                ? GetRandomPhoneNumber()
                : phoneNumber.ToString();

            var phoneResult = Phone.CreateNotEmpty(newPhoneNumber, VALID_PHONE_CODE);

            if (phoneResult.IsFailure)
            {
                newPhoneNumber = GetRandomPhoneNumber();

                phoneResult = Phone.CreateNotEmpty(newPhoneNumber, VALID_PHONE_CODE);

                throw new Exception($"Cant create volunteer with builder!Error:" +
                    $"{phoneResult.ValidationMessagesToString()}");
            }
            var randomFirstName = GetRandomName(4, 10);

            var randomLastName = GetRandomName(5, 10);

            var phone = phoneResult.Data!;

            var result = Volunteer.Create(
                VolunteerID.NewGuid(),
                UserId.NewGuid(),
                FullName.Create(randomFirstName, randomLastName).Data!,
                 1,
                "test description",
                phone,
                []);

            if (result.IsFailure)
                throw new Exception($"Cant create volunteer with builder!Error:" +
                    $"{result.ValidationMessagesToString()}");

            Volunteers.Add(result.Data!);
        }
    }

    public static List<Volunteer> Build(
        int volunteersCount,
        int volunteerPetsCount,
        List<Species> speciesList)
    {
        var volunteerBuilder = new VolunteerTestBuilder(speciesList);
        for (int i = 0; i < volunteersCount; i++)
        {
            var volunteer = volunteerBuilder.CreateVolunteerWithRandomData(i);

            for (int j = 0; j < volunteerPetsCount; j++)
            {
                volunteerBuilder.AddRandomPetToVolunteer(j, volunteer, speciesList);
            }

            volunteerBuilder.Volunteers.Add(volunteer);
        }
        return volunteerBuilder.Volunteers;
    }

    private Pet AddRandomPetToVolunteer(
       int j,
       Volunteer volunteer,
       List<Species> speciesList)
    {
        var randomName = GetRandomName(3, 8);
        var randomBirthday = GetRandomBirthDateForPet();
        var randomSpecies = speciesList[Random.Shared.Next(0, speciesList.Count)];
        var randomBreed = randomSpecies.Breeds[Random.Shared.Next(0, randomSpecies.Breeds.Count)];
        var randomAddress = RandomAddressGenerator.GenerateRandomAddress();
        var randomHelpStatus = RandomEnumGenerator.GetRandomValue<HelpStatus>();

        var pet = volunteer.CreateAndAddPet(
           randomName,
           randomBirthday,
           "description",
           GetRandomBool(),
           GetRandomBool(),
           Random.Shared.Next(1, 10),
           Random.Shared.Next(1, 10),
           Colors[Random.Shared.Next(0, Colors.Length)],
           PetType.Create(
               BreedID.SetValue(randomBreed.Id),
               SpeciesID.SetValue(randomSpecies.Id)).Data!,
           Phone.CreateEmpty(),
           [],
           randomHelpStatus,
           "health info",
           randomAddress);

        if (pet == null)
            throw new Exception($"Cant create and add pet to volunteer with builder!");

        for (int i = 0; i < 9; i++)
            pet.AddImages([$"{Guid.NewGuid().ToString()}" + ".jpg"]);

        return pet;
    }
    public VolunteerTestBuilder WithPets(int petsCount, Species species)
    {
        foreach (var volunteer in Volunteers)
        {
            for (int i = 0; i < petsCount; i++)
            {
                volunteer.CreateAndAddPet(
                    GetRandomName(3, 10),
                    GetRandomBirthDateForPet(),
                    "description",
                    GetRandomBool(),
                    GetRandomBool(),
                    Random.Shared.Next(1, 10),
                    Random.Shared.Next(1, 10),
                    Colors[Random.Shared.Next(0, Colors.Length)],
                    PetType.Create(
                        BreedID.SetValue(species.Breeds[0].Id),
                        SpeciesID.SetValue(species.Id)).Data!,
                    Phone.CreateEmpty(),
                    [],
                    GetRandomValue<HelpStatus>(),
                    "health info",
                    Address.CreateEmpty());
            }
        }
        return this;
    }

    public List<Volunteer> GetVolunteers() => Volunteers;
    public Volunteer GetVolunteer() => Volunteer;

    public static string GetRandomName(int minLength, int maxLength)
    {
        var randomLength = Random.Shared.Next(minLength, maxLength);
        var result = new char[randomLength];
        for (int i = 0; i < randomLength; i++)
        {
            result[i] = chars[Random.Shared.Next(chars.Length)];
            if (i == 0)
                result[i] = char.ToUpperInvariant(result[i]);
            else
                result[i] = char.ToLowerInvariant(result[i]);
        }

        return new string(result);
    }

    public static string GetRandomPhoneNumber()
    {
        var randomNumber = Random.Shared.Next(4000000, 99999999);
        return randomNumber.ToString();
    }

    public static DateOnly GetRandomBirthDateForPet()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var maxDate = today.AddYears(-0).AddMonths(-0);
        var minDate = today.AddYears(-10);

        int range = maxDate.DayNumber - minDate.DayNumber;
        int randomDays = Random.Shared.Next(range + 1);

        return minDate.AddDays(randomDays);
    }

    public static bool GetRandomBool()
    {
        return Random.Shared.Next(2) == 0;
    }

    private Volunteer CreateVolunteerWithRandomData(int i)
    {
        var firstName = GetRandomName(4, 10);
        var lastName = GetRandomName(5, 10);

        var randomEmail = $"{firstName}" + "-" + $"{lastName}" + "@gmail.com";

        var randomPhoneNumber = Random.Shared.Next(10000000, 100000000).ToString();
        var randomPhoneRegion = "+" + Random.Shared.Next(20, 400).ToString();

        var phoneResult = Phone.CreateNotEmpty(randomPhoneNumber, randomPhoneRegion);

        if (phoneResult.IsFailure)
            throw new Exception($"Cant create volunteer with builder!Error:" +
                $"{phoneResult.ValidationMessagesToString()}");

        var phone = phoneResult.Data!;  

        var volunteerResult = Volunteer.Create(
            VolunteerID.NewGuid(),
            UserId.NewGuid(),
            FullName.Create(firstName, lastName).Data!,
            Random.Shared.Next(1, 25),
            "test description",
            phone,
            []);

        if (volunteerResult.IsFailure)
            throw new Exception($"Cant create volunteer with builder!Error:" +
                $"{volunteerResult.ValidationMessagesToString()}");

        return volunteerResult.Data!;
    }
}
