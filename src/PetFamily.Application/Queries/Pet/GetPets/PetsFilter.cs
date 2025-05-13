using Microsoft.AspNetCore.Mvc;
using PetFamily.Domain.PetManagment.Enums;
using System.Text.Json.Serialization;

namespace PetFamily.Application.Queries.Pet.GetPets;

public class  PetsFilter
{
    public Guid? VolunteerId { get; set; }
    public List<Guid>? SpeciesIds { get; set; }
    public List<Guid>? BreedIds { get; set; }
    public string? PetName { get; set; }
    public string? City { get; set; }
    public string? Color { get; set; }
    public List<HelpStatus>? HelpStatuses { get; set; }
    public int MinAgeInMonth { get; set; } = 0;
    public int MaxAgeInMonth { get; set; } = 0;
    public List<string>? OrderBy { get; set; }

    public List<OrderBy> GetOrderBies()=>
        OrderBy?.Select(x =>
        {
            var parts = x.Split(':');
            return new OrderBy(
                parts.ElementAtOrDefault(0) ?? "",
                parts.ElementAtOrDefault(1)?.ToLower() == "desc" ? OrderDirection.Desc : OrderDirection.Asc);
        }).ToList() ?? new();
}
