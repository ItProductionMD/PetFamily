using PetFamily.Application.Abstractions.CQRS;
using PetFamily.Application.Dtos;

namespace PetSpecies.Application.Queries.GetSpeciesPagedList;


public class GetSpeciesPagedListQuery : IQuery
{
    public SpeciesFilterDto? SpeciesFilter { get; set; }
    public PaginationDto? Pagination { get; set; }

    public GetSpeciesPagedListQuery(SpeciesFilterDto? speciesFilterDto, PaginationDto? pagination)
    {
        SpeciesFilter = speciesFilterDto;
        Pagination = pagination;
    }
}

