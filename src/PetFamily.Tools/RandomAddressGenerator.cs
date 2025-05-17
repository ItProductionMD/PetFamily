using PetFamily.Domain.Shared.ValueObjects;

namespace PetFamily.Tools;

/// <summary>
/// This class generates random addresses for testing purposes.
/// </summary>
public static class RandomAddressGenerator
{
    private static readonly Dictionary<string, List<string>> StatesAndCities = new()
    {
        { "California", new() { "Los Angeles", "San Francisco", "San Diego" } },
        { "Texas", new() { "Houston", "Dallas", "Austin" } },
        { "Florida", new() { "Miami", "Orlando", "Tampa" } },
        { "New York", new() { "New York", "Buffalo", "Rochester" } },
        { "Illinois", new() { "Chicago", "Springfield", "Naperville" } },
        { "Georgia", new() { "Atlanta", "Savannah", "Augusta" } },
        { "Ohio", new() { "Columbus", "Cleveland", "Cincinnati" } },
        { "North Carolina", new() { "Charlotte", "Raleigh", "Greensboro" } },
        { "Pennsylvania", new() { "Philadelphia", "Pittsburgh", "Allentown" } },
        { "Arizona", new() { "Phoenix", "Tucson", "Scottsdale" } }
    };

    private static readonly List<string> StreetNames =
    [
        "Broadway", "Elm St", "Main St", "Maple Ave", "Chestnut St",
        "5th Ave", "Sunset Blvd", "Wilson Ave", "Taylor St", "Madison Ave"
    ];

    private static readonly Random Random = new();

    public static Address GenerateRandomAddress()
    {
        var state = GetRandomKey(StatesAndCities);
        var city = GetRandomItem(StatesAndCities[state]);
        var street = GetRandomItem(StreetNames);
        var homeNumber = GenerateRandomHomeNumber();

        var addressResult = Address.CreatePossibleEmpty(
            street: street,
            number: homeNumber,
            city: city,
            region: state
        );

        if (addressResult.IsFailure)
        {
            throw new InvalidOperationException($"Failed to generate a valid address.Error:" +
                $"{addressResult.ValidationMessagesToString()}");
        }

        return addressResult.Data!;
    }

    private static string GetRandomItem<T>(List<T> list)
    {
        return list[Random.Next(list.Count)]!.ToString();
    }

    private static string GetRandomKey<TKey, TValue>(Dictionary<TKey, TValue> dict)
    {
        var keys = new List<TKey>(dict.Keys);
        return keys[Random.Next(keys.Count)]!.ToString();
    }

    private static string GenerateRandomHomeNumber()
    {
        int baseNumber = Random.Next(1, 200);
        string[] suffixes = { "", "A", "B", "/1", "/2", "/3" };
        return baseNumber + GetRandomItem(new List<string>(suffixes));
    }
}
