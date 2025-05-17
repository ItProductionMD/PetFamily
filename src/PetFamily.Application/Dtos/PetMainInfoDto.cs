namespace PetFamily.Application.Dtos;

public class PetMainInfoDto
{
    public Guid PetId { get; set; }
    public string PetName { get; set; }
    public string? MainPhoto { get; set; }
    public string StatusForHelp { get; set; }
    public string BreedName { get; set; }
    public string SpeciesName { get; set; }

    public PetMainInfoDto(Guid id, string name, string mainPhoto, string statusForHelp, string speciesName, string breedName)
    {
        PetId = id;
        PetName = name;
        MainPhoto = mainPhoto;
        StatusForHelp = statusForHelp;
        SpeciesName = speciesName;
        BreedName = breedName;
    }
}

