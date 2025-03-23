using System.Runtime.CompilerServices;

namespace PetFamily.Domain.Shared.Interfaces;

public interface IHasUniqueFields
{
    static abstract string[] GetUniqueFields();
}
