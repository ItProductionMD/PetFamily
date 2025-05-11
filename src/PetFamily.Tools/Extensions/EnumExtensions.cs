namespace PetFamily.Tools.Extensions;

public class EnumExtensions
{
    private static readonly Random _random = new();

    public static T GetRandomValue<T>() where T : Enum
    {
        var values = Enum.GetValues(typeof(T));
        return (T)values.GetValue(_random.Next(values.Length))!;
    }
}
