using PetFamily.SharedKernel.Abstractions;
using System.Runtime.InteropServices;

namespace PetFamily.SharedKernel.ValueObjects;

public readonly record struct Id<T>(Guid Value) where T : Entity<Id<T>>
{
    public static Id<T> Create(Guid value) => new(value);
    public static Id<T> New() => new(Guid.NewGuid());
    public static Id<T> Empty => new(Guid.Empty);
    public override string ToString() => Value.ToString();
}