using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PetFamily.Infrastructure.Configurations;

public static class Converters
{
    public class ReadOnlyListConverter<T> : ValueConverter<IReadOnlyList<T>, string>
    {
        public ReadOnlyListConverter()
            : base(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer
                .Deserialize<IReadOnlyList<T>>(v, (JsonSerializerOptions?)null) ?? new List<T>())
        { }
    }
    
}