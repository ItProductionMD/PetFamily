using PetFamily.SharedApplication.Dtos;
using PetFamily.SharedApplication.Enums;

namespace PetSpecies.Application.Queries.GetSpeciesPagedList;

public class SpeciesFilterDto
{
    public List<SpeciesSearchParams>? SearchNames { get; set; }
    public List<string>? OrderByes { get; set; }
    public List<OrderBy<SpeciesSortProperty>>? MappedOrderByes { get; set; }
    public List<OrderBy<SpeciesSortProperty>> GetOrderByes()
    {
        if (OrderByes == null)
            return new List<OrderBy<SpeciesSortProperty>>();

        var result = new List<OrderBy<SpeciesSortProperty>>();

        foreach (var x in OrderByes)
        {
            var parts = x.Split(':');
            if (parts.Length == 0)
                continue;

            if (Enum.TryParse<SpeciesSortProperty>(parts[0], out var property))
            {
                var direction = OrderDirection.Asc;

                if (parts.Length > 1 && parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase))
                    direction = OrderDirection.Desc;

                result.Add(new OrderBy<SpeciesSortProperty>(property, direction));
            }
        }

        return result;
    }
}

public class SpeciesSearchParams
{
    public SpeciesSearchProperty SearchProperty { get; set; }
    public string SearchName { get; set; }
}

public enum SpeciesSearchProperty
{
    name
}
public enum SpeciesSortProperty
{
    name,
    id
}
