using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.Domain.Shared;
using System.Reflection;

namespace PetFamily.Infrastructure.Contexts;

public static class ModelBuilderExtensions
{
    public static void ApplyUniqueConstraints<T>(this EntityTypeBuilder<T> builder) where T : class
    {
        foreach (var property in typeof(T).GetProperties())
        {
            if (property.GetCustomAttribute<UniqueAttribute>() == null)
                continue;

            if (property.PropertyType == typeof(string) || property.PropertyType.IsValueType)
            {
                Console.Write($"Set unique index for {property.Name}:...");
                builder.HasIndex(property.Name).IsUnique();
                Console.WriteLine("OK!");
            }

            if (IsComplexProperty(property.PropertyType))
            {
                builder.OwnsOne(property.PropertyType, property.Name, ownedBuilder =>
                {
                    var uniqueProps = property.PropertyType.GetProperties()
                        .Select(p => p.Name)
                        .ToArray();

                    var uniqueIndexString = string.Join(",", uniqueProps);
                    Console.Write($"Set index for OwnsOne property:'{property.Name}'" +
                        $" for columns:'{uniqueIndexString}'...");
                    if (uniqueProps.Length > 0)
                    {
                        ownedBuilder.HasIndex(uniqueProps).IsUnique();
                    }
                    Console.WriteLine("Ok!");
                });
            }
        }
    }

    private static bool IsComplexProperty(Type type) =>
        type.IsClass && type.GetMethod("GetEqualityComponents") != null;
}