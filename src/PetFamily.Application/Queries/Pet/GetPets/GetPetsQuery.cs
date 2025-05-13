using PetFamily.Application.Abstractions;

namespace PetFamily.Application.Queries.Pet.GetPets;

public record GetPetsQuery(
    int PageNumber,
    int PageSize,
    PetsFilter? PetsFilter = null):IQuery;
