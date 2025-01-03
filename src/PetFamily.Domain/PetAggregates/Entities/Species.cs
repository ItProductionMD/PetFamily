using PetFamily.Domain.DomainResult;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.Validations;

namespace PetFamily.Domain.PetAggregates.Entities;

public class Species : Entity<int>
{
    public string Name { get; private set; }

    private readonly List<Breed> _breeds = [];
    public IReadOnlyList<Breed> Breeds => _breeds;

    private Species(int id) : base(id) { }//Ef core needs this
    private Species(int id, string name) : base(id)
    {
        Name = name;
    }
    public void AddBreed(Breed breed)=>_breeds.Add(breed);
    public int GetBreedCount()=>_breeds.Count;
    public static Result<Species> Create(int id, string? name)
    {
        var validationResult = Validate(name);
        if (validationResult.IsFailure)
            return Result<Species>.Failure(validationResult.Errors);
        return Result<Species>.Success(new Species(id, name!));
    }
    public static Result Validate(string? name)
    {
        var nullOrEmptyValidationResult = ValidationExtensions.ValidateIfStringNotEmpty("Name", name);
        if(nullOrEmptyValidationResult.IsFailure)
            return Result.Failure(nullOrEmptyValidationResult.Errors);
        //TODO: Add more validations    
        return Result.Success();
    }

}
