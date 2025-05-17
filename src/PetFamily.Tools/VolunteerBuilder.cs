using PetFamily.Domain.PetManagment.Entities;
using PetFamily.Domain.PetManagment.Enums;
using PetFamily.Domain.PetManagment.Root;
using PetFamily.Domain.PetManagment.ValueObjects;
using PetFamily.Domain.PetTypeManagment.Root;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Tools.Extensions;
using System.Text;

namespace PetFamily.Tools;

public class VolunteerBuilder
{
    public List<Volunteer> Volunteers { get; set; } = [];
    public const string VALID_EMAIL = "email@gmail.com";
    public const int VALID_PHONE_NUMBER = 6111111;
    public const string VALID_PHONE_CODE = "+373";
    public string[] Colors = ["black", "red", "gray", "yellow", "brown", "green", "white"];
    public const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    public Random Random = new Random();
    private List<Species> speciesList;

    private VolunteerBuilder(List<Domain.PetTypeManagment.Root.Species> speciesList)
    {
        this.speciesList = speciesList;
    }

    public static List<Volunteer> Build(
        int volunteersCount,
        int volunteerPetsCount,
        List<Species> speciesList)
    {

        var volunteerBuilder = new VolunteerBuilder(speciesList);
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

    private Volunteer CreateVolunteerWithRandomData(int i)
    {
        var volunteerNameBuilder = new StringBuilder();
        var reverseBuilder = new StringBuilder();

        for (int j = 0; j < 10; j++)
            volunteerNameBuilder.Append(chars[Random.Next(chars.Length)]);

        string firstName = volunteerNameBuilder.ToString().ToLower();

        string lastName = new string(firstName.ToCharArray().Reverse().ToArray()).ToLower();

        var fakeEmail = $"{firstName}" + "-" + $"{lastName}" + "@gmail.com";

        var randomNumber = new Random();
        var randomPhoneNumber = randomNumber.Next(10000000, 100000000).ToString();
        string randomPhoneRegion = "+" + randomNumber.Next(20, 400).ToString();

        var phoneResult = Phone.CreateNotEmpty(randomPhoneNumber, randomPhoneRegion);
        if (phoneResult.IsFailure)
            throw new Exception($"Cant create volunteer with builder!Error:" +
                $"{phoneResult.ValidationMessagesToString()}");

        var volunteerResult = Volunteer.Create(
            VolunteerID.NewGuid(),
            FullName.Create(firstName, lastName).Data!,
            fakeEmail,
            phoneResult.Data!,
            i > 50 ? 1 : i,
            "test description",
            [],
            []
        );

        if (volunteerResult.IsFailure)
            throw new Exception($"Cant create volunteer with builder!Error:" +
                $"{volunteerResult.ValidationMessagesToString()}");

        return volunteerResult.Data!;
    }

    private Pet AddRandomPetToVolunteer(
        int j,
        Volunteer volunteer,
        List<Species> speciesList)
    {
        var petNameBuilder = new StringBuilder();
        for (int k = 0; k < 7; k++)
            petNameBuilder.Append(chars[Random.Next(chars.Length)]);

        string randomPetName = petNameBuilder.ToString().ToLower();

        var datetimeNow = DateTime.Now;
        var date = datetimeNow.AddMonths(Random.Next(-50, -1));

        var randomSpecies = speciesList[Random.Next(0, speciesList.Count)];
        var randomBreed = randomSpecies.Breeds[Random.Next(0, randomSpecies.Breeds.Count)];

        var randomAddress = RandomAddressGenerator.GenerateRandomAddress();

        var randomHelpStatus = EnumExtensions.GetRandomValue<HelpStatus>();

        var pet = volunteer.CreateAndAddPet(
           randomPetName,
           DateOnly.FromDateTime(date),
           "description",
           Random.Next(2) == 0,
           Random.Next(2) == 0,
           Random.Next(0, 10),
           Random.Next(0, 5),
           Colors[Random.Next(0, Colors.Length)],
           PetType.Create(
               BreedID.SetValue(randomBreed.Id),
               SpeciesID.SetValue(randomSpecies.Id)).Data!,
           volunteer.Phone,
           [],
           randomHelpStatus,
           "health info",
           randomAddress);

        for (int i = 0; i < 9; i++)
            pet.AddImages([$"{Guid.NewGuid().ToString()}" + ".jpg"]);

        if (pet == null)
            throw new Exception($"Cant create and add pet to volunteer with builder!");

        return pet;
    }

    public static string GetRandomName(int minLength, int maxLength)
    {
        var random = new Random();
        var randomLength = random.Next(minLength, maxLength);
        var result = new char[randomLength];
        for (int i = 0; i < randomLength; i++)
        {
            result[i] = chars[random.Next(chars.Length)];
            if (i == 0)
                result[i] = char.ToUpperInvariant(result[i]);
            else
                result[i] = char.ToLowerInvariant(result[i]);
        }

        return new string(result);
    }
}

