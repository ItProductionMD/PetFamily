using PetFamily.Domain.DomainResult;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.Validations;

namespace PetFamily.Domain.PetAggregates.Entities;

public class Breed : Entity<Guid>
{
    public string Name { get; private set; }
    public string? Description { get; private set; }
    private Breed(Guid id) : base(id) { }//Ef core needs this
    private Breed(Guid id,string name, string? description):base(id)
    {
        Name = name;
        Description = description;
    }
    public static Result<Breed> Create(Guid id,string? name, string? description)
    {
        var validationResult = Validate(name, description);
        if (validationResult.IsFailure)
            return Result<Breed>.Failure(validationResult.Errors);
        return Result<Breed>.Success(new Breed(id,name!, description!) );
    }
    
    public static Result Validate(string? name,string? description)
    {
        var validateName=ValidationExtensions.ValidateIfStringNotEmpty("Name",name);
        if (validateName.IsFailure)
            return Result.Failure(validateName.Errors);
        //TODO: Add more validations
        return Result.Success();
    }
}
