using PetFamily.Application.Abstractions.CQRS;
using PetFamily.Application.Dtos;

namespace PetSpecies.Application.Queries.GetBreedPagedList;

public class GetBreedPageListQuery : IQuery
{
    public PaginationDto Pagination { get; set; }
    public BreedFilterDto? BreedFilter { get; set; }
    public GetBreedPageListQuery(PaginationDto? pagination, BreedFilterDto? breedFilter)
    {
        Pagination = pagination ?? new(1, 20);
        BreedFilter = breedFilter;
    }
}
