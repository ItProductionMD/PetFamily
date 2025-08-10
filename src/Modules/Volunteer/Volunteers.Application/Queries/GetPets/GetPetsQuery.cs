using PetFamily.SharedApplication.Abstractions.CQRS;
using Volunteers.Application.Queries.GetPets.ForFilter;

namespace Volunteers.Application.Queries.GetPets;

public record GetPetsQuery(
    int PageNumber,
    int PageSize,
    PetsFilter? PetsFilter = null) : IQuery;
