using System.Globalization;

namespace PetFamily.Application.Queries.ForFilters;

public record OrderBy(string OrderByProperty, OrderDirection OrderDirection);

