﻿using PetFamily.SharedKernel.Abstractions;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects;
using static PetFamily.SharedKernel.Validations.ValidationConstants;
using static PetFamily.SharedKernel.Validations.ValidationExtensions;
using static PetFamily.SharedKernel.Validations.ValidationPatterns;

namespace PetSpecies.Domain;

public class Species : IEntity<Guid>
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }

    private List<Breed> _breeds = [];
    public IReadOnlyList<Breed> Breeds => _breeds;

    private Species()  { }//Ef core needs this

    private Species(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public void AddBreed(Breed breed) => _breeds.Add(breed);

    public int GetBreedCount() => _breeds.Count;

    public static Result<Species> Create(SpeciesID id, string? name)
    {
        var validationResult = Validate(name);
        if (validationResult.IsFailure)
            return validationResult;

        return Result.Ok(new Species(id.Value, name!));
    }
    public static UnitResult Validate(string? name) =>

        ValidateRequiredField(name, "Species name", MAX_LENGTH_SHORT_TEXT, NAME_PATTERN);

    public void AddBreeds(List<Breed> breeds)
    {
        _breeds.AddRange(breeds);
    }

    public UnitResult DeleteBreedsById(List<Guid> breedIdsToDelete)
    {
        var removedItemsCount = _breeds.RemoveAll(b => breedIdsToDelete.Contains(b.Id));
        if (removedItemsCount == 0)
            return UnitResult.Fail(Error.NotFound(
                $"Breeds with Ids: {string.Join(',', breedIdsToDelete)} not found"));

        return UnitResult.Ok();
    }

    public UnitResult DeleteBreedById(Guid breedId)
    {
        var removedItemsCount = _breeds.RemoveAll(b => b.Id == breedId);
        if (removedItemsCount == 0)
            return UnitResult.Fail(Error.NotFound($"Breed with Id: {breedId} not found"));

        return UnitResult.Ok();
    }

}