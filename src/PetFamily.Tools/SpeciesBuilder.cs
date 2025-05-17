using PetFamily.Domain.PetManagment.ValueObjects;
using PetFamily.Domain.PetTypeManagment.Entities;
using PetFamily.Domain.PetTypeManagment.Root;

namespace PetFamily.Tools;

public class SpeciesBuilder
{
    public static readonly Dictionary<string, string[]> SpeciesAndBreeds = new()
    {
        { "cat", new[] { "British", "Sphynx", "MaineCoon" } },
        { "dog", new[] { "Labrador", "Poodle", "Bulldog" } },
        { "rodent", new[] { "Hamster", "GuineaPig", "Chinchilla" } }
    };
    private List<Species> SpeciesList { get; set; } = [];
    public static List<Species> Build()
    {
        var speciesBuilder = new SpeciesBuilder();
        foreach (var species in SpeciesAndBreeds)
        {
            var speciesResult = Species.Create(SpeciesID.SetValue(Guid.NewGuid()), species.Key);
            if (speciesResult.IsFailure)
                throw new Exception($"Can't create species with test builder!Error:" +
                    $"{speciesResult.ValidationMessagesToString()}");

            var newSpecies = speciesResult.Data!;
            foreach (var breedName in species.Value)
            {
                var breedResult = Breed.Create(
                    BreedID.SetValue(Guid.Empty),
                    breedName,
                    "test breed description");

                if (breedResult.IsFailure)
                    throw new Exception($"Can't create breed with test builder!Error:" +
                        $"{breedResult.ValidationMessagesToString()}");

                newSpecies.AddBreed(breedResult.Data!);
            }
            speciesBuilder.SpeciesList.Add(newSpecies);
        }
        return speciesBuilder.SpeciesList;
    }

}
