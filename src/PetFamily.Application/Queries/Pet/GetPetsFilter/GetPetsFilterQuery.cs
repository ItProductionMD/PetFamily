using PetFamily.Application.Abstractions;

namespace PetFamily.Application.Queries.Pet.GetPetsFilter;

public record GetPetsFilterQuery(bool HasFilter) :IQuery;
