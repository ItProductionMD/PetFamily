using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PetFamily.SharedKernel.Abstractions;
using PetFamily.SharedKernel.ValueObjects;
using System.Text.Json;

namespace PetFamily.SharedInfrastructure.Shared.EFCore;

public static class Convertors
{
    public class ReadOnlyListConverter<T> : ValueConverter<IReadOnlyList<T>, string>
    {
        public ReadOnlyListConverter() : base(
            v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
            v => JsonSerializer.Deserialize<IReadOnlyList<T>>(v, JsonSerializerOptions.Default) ?? new List<T>())
        { }
    }
}