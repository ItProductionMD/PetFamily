using Dapper;
using PetFamily.Application.Queries.Pet.GetPets;
using PetFamily.Infrastructure.Dapper.GeneratedTables;
using System.Text;

namespace PetFamily.Infrastructure.Extensions;

public static class StringBuilderExtensions
{
    internal static StringBuilder AppendJoinsForGetPets(this StringBuilder countBuilder, PetsFilter filter)
    {
        if (filter.VolunteerId.HasValue)
            countBuilder.Append($" LEFT JOIN {Volunteers.Table} v " +
                $"ON v.{Volunteers.Id} = p.{Pets.VolunteerId}");

        if (filter.SpeciesIds != null && filter.SpeciesIds.Count > 0)
            countBuilder.Append($" LEFT JOIN {Species.Table} s " +
                $"ON s.{Species.Id} = p.{Pets.PetTypeSpeciesId}");

        if (filter.BreedIds != null && filter.BreedIds.Count > 0)
            countBuilder.Append($" LEFT JOIN {Breeds.Table} b " +
                $"ON b.{Breeds.Id} = p.{Pets.PetTypeBreedId}");

        countBuilder.AppendLine($" WHERE p.{Pets.IsDeleted} = FALSE ");

        return countBuilder;
    }

    internal static StringBuilder AppendFiltersForGetPets(
        this StringBuilder sqlBuilder,
        DynamicParameters parameters,
        PetsFilter filter)
    {
        if (filter.VolunteerId.HasValue)
        {
            sqlBuilder.AppendLine($" AND p.{Pets.VolunteerId} = @VolunteerId");

            if (parameters.ParameterNames.Contains("VolunteerId") == false)
                parameters.Add("VolunteerId", filter.VolunteerId.Value);
        }

        if (filter.HelpStatuses !=null && filter.HelpStatuses.Any())
        {
            List<string> statusStrings = filter.HelpStatuses
                .Select(s => s.ToString())
                .ToList();

            sqlBuilder.AppendLine($" AND p.{Pets.HelpStatus} = Any(@PetHelpStatuses)");

            if (parameters.ParameterNames.Contains("PetHelpStatus") == false)
                parameters.Add("PetHelpStatuses", statusStrings);
        }

        if (!string.IsNullOrWhiteSpace(filter.PetName))
        {
            sqlBuilder.AppendLine($" AND LOWER(p.{Pets.Name}) LIKE LOWER(@PetName)");

            if (parameters.ParameterNames.Contains("PetName") == false)
                parameters.Add("PetName", $"%{filter.PetName}%");
        }

        if (!string.IsNullOrWhiteSpace(filter.Color))
        {
            sqlBuilder.AppendLine($" AND LOWER(p.{Pets.Color}) LIKE LOWER(@Color)");

            if (parameters.ParameterNames.Contains("Color") == false)
                parameters.Add("Color", $"%{filter.Color}%");
        }

        if (!string.IsNullOrWhiteSpace(filter.City))
        {
            sqlBuilder.AppendLine($" AND LOWER(p.{Pets.AddressCity}) LIKE LOWER(@City)");
            parameters.Add("City", $"%{filter.City}%");
        }

        if(filter.SpeciesIds != null && filter.SpeciesIds.Any()
            ||filter.BreedIds!=null && filter.BreedIds.Any())
        {
            sqlBuilder.AppendLine(" AND (");
            string or = string.Empty;
            if (filter.SpeciesIds != null && filter.SpeciesIds.Any())
            {
                sqlBuilder.AppendLine($"p.{Pets.PetTypeSpeciesId} = ANY(@SpeciesIds)");
                parameters.Add($"SpeciesIds", filter.SpeciesIds);
                or = " OR ";
            }

            if (filter.BreedIds != null && filter.BreedIds.Any())
            {
                sqlBuilder.AppendLine($" {or} p.{Pets.PetTypeBreedId} = ANY(@BreedIds)");
                parameters.Add($"BreedIds", filter.BreedIds);
            }
            sqlBuilder.AppendLine(" )");
        }

        if (filter.MinAgeInMonth > 0)
        {
            var currentDate = DateTime.UtcNow;
            var maxDateOfBirthday = currentDate.AddMonths(-filter.MinAgeInMonth);
            sqlBuilder.AppendLine($" AND p.{Pets.DateOfBirth} <= @MaxDate");

            if (parameters.ParameterNames.Contains("MaxDate") == false)
                parameters.Add("MaxDate", maxDateOfBirthday);
        }

        if (filter.MaxAgeInMonth > 0)
        {
            var currentDate = DateTime.UtcNow;
            var minDateOfBirthday = currentDate.AddMonths(-filter.MaxAgeInMonth);
            sqlBuilder.AppendLine($" AND p.{Pets.DateOfBirth} >= @MinDate");

            if (parameters.ParameterNames.Contains("MinDate") == false)
                parameters.Add("MinDate", minDateOfBirthday);
        }
        return sqlBuilder;
    }

    internal static StringBuilder AppendPagination(
        this StringBuilder sqlBuilder,
        int pageNumber,
        int pageSize,
        DynamicParameters parameters)
    {
        sqlBuilder.AppendLine(" LIMIT @PageSize OFFSET @Offset");
        parameters.Add("Offset", (pageNumber - 1) * pageSize);
        parameters.Add("PageSize", pageSize);
        return sqlBuilder;
    }

    internal static StringBuilder AppendOrderByForGetPets(this StringBuilder sql, List<OrderBy> orderBies)
    {
        if (orderBies == null || orderBies.Count == 0)
        {
            sql.AppendLine($" ORDER BY p.{Pets.Id}");
            return sql;
        }
        var orderClauses = orderBies.Select(o =>
        {
            var direction = o.OrderDirection == OrderDirection.Desc ? "DESC" : "ASC";
            var column = o.OrderByProperty.ToLower() switch
            {
                "name" => $"p.{Pets.Name} {direction}",

                "age" => $"p.{Pets.DateOfBirth} {direction}",

                "volunteer" => $"v.{Volunteers.LastName} {direction}," +
                               $"v.{Volunteers.FirstName} {direction}",

                "species" => $"s.{Species.Name} {direction}, " +
                           $"b.{Breeds.Name} {direction}",

                "status" => $"p.{Pets.HelpStatus} {direction}",

                "address" => $"p.{Pets.AddressCity} {direction}, " +
                           $"p.{Pets.AddressRegion} {direction}," +
                           $"p.{Pets.AddressStreet} {direction}",

                _ => $"p.{Pets.Id} {direction}"
            };
            return $"{column}";
        });

        sql.AppendLine(" ORDER BY " + string.Join(", ", orderClauses));

        return sql;
    }
}
