using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace PetFamily.Infrastructure.WriteDbConfigurations;

public static class Converters
{
    public class ReadOnlyListConverter<T> : ValueConverter<IReadOnlyList<T>, string>
    {
        public ReadOnlyListConverter() : base(
            v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
            v => JsonSerializer.Deserialize<IReadOnlyList<T>>(v, JsonSerializerOptions.Default) ?? new List<T>())
        { }
    }

}