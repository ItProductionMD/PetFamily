using PetFamily.SharedKernel.ValueObjects;
using PetSpecies.Domain;

namespace PetFamily.IntegrationTests.TestData;

public class SpeciesTestBuilder
{
    public Species Species { get; set; }
    public List<Breed>? Breeds { get; set; }
    public const string SPECIES_NAME = "testSpecies";

    public SpeciesTestBuilder(string? speciesName = null)
    {
        var name = string.IsNullOrWhiteSpace(speciesName) ? SPECIES_NAME : speciesName;
        var speciesResult = Species.Create(SpeciesID.SetValue(Guid.NewGuid()), name);
        if (speciesResult.IsFailure)
            throw new Exception($"Can't create species with test builder!Error:" +
                $"{speciesResult.ValidationMessagesToString()}");

        Species = speciesResult.Data!;
    }

    public SpeciesTestBuilder WithBreeds(List<string> breedNames)
    {
        foreach (var breedName in breedNames)
        {
            var breedResult = Breed.Create(
                BreedID.SetValue(Guid.Empty),
                breedName,
                "test breed description");
            if (breedResult.IsFailure)
                throw new Exception($"Can't create breed with test builder!Error:" +
                    $"{breedResult.ValidationMessagesToString()}");

            Species.AddBreed(breedResult.Data!);
        }
        return this;
    }

    public Species GetSpecies() => Species;
}
