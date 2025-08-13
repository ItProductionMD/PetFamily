using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetFamily.SharedKernel.Uniqness;
using System.Reflection;

namespace PetFamily.SharedInfrastructure.Shared.EFCore;

public static class ModelBuilderExtensions
{
    public static void ApplyUniqueConstraintsFromAttributes(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;

            // Создаём EntityTypeBuilder<T> с помощью рефлексии
            var method = typeof(ModelBuilderExtensions)
                .GetMethod(nameof(ApplyUniqueConstraints), BindingFlags.Static | BindingFlags.NonPublic)!
                .MakeGenericMethod(clrType);

            var entityBuilder = modelBuilder.Entity(clrType);
            method.Invoke(null, new object[] { entityBuilder });
        }
    }

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

    public static void ApplyIdConversions(this ModelBuilder modelBuilder)
    {
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            var clrType = entity.ClrType;
            var builder = modelBuilder.Entity(clrType);
        }
    }

    private static bool IsComplexProperty(Type type) =>
        type.IsClass && type.GetMethod("GetEqualityComponents") != null;
}