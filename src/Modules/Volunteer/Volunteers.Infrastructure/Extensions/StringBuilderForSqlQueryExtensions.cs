using Dapper;
using PetFamily.SharedInfrastructure.Dapper.ScaffoldedClasses;
using System.Text;
using Volunteers.Application.Queries.GetPets.ForFilter;

namespace Volunteers.Infrastructure.Extensions;

public static class StringBuilderForSqlQueryExtensions
{
    internal static StringBuilder AppendJoinsForGetPets(
        this StringBuilder countBuilder,
        PetsFilter filter)
    {
        if (filter.VolunteerId.HasValue)
            countBuilder.Append($" LEFT JOIN {VolunteersTable.TableFullName} v " +
                $"ON v.{VolunteersTable.Id} = p.{PetsTable.VolunteerId}");

        if (filter.SpeciesIds != null && filter.SpeciesIds.Count > 0)
            countBuilder.Append($" LEFT JOIN {SpeciesTable.TableFullName} s " +
                $"ON s.{SpeciesTable.Id} = p.{PetsTable.PetTypeSpeciesId}");

        if (filter.BreedIds != null && filter.BreedIds.Count > 0)
            countBuilder.Append($" LEFT JOIN {BreedsTable.TableFullName} b " +
                $"ON b.{BreedsTable.Id} = p.{PetsTable.PetTypeBreedId}");

        countBuilder.AppendLine($" WHERE p.{PetsTable.IsDeleted} = FALSE ");

        return countBuilder;
    }

    internal static StringBuilder AppendFiltersForGetPets(
        this StringBuilder sqlBuilder,
        DynamicParameters parameters,
        PetsFilter filter)
    {
        if (filter.VolunteerId.HasValue)
        {
            sqlBuilder.AppendLine($" AND p.{PetsTable.VolunteerId} = @VolunteerId");

            if (parameters.ParameterNames.Contains("VolunteerId") == false)
                parameters.Add("VolunteerId", filter.VolunteerId.Value);
        }

        if (filter.HelpStatuses != null && filter.HelpStatuses.Any())
        {
            string[] statusStrings = filter.HelpStatuses
                .Select(s => s.ToString())
                .ToArray();

            sqlBuilder.AppendLine($" AND p.{PetsTable.HelpStatus} = ANY(@PetHelpStatuses) ");

            if (parameters.ParameterNames.Contains("PetHelpStatuses") == false)
                parameters.Add("PetHelpStatuses", statusStrings);
        }

        if (!string.IsNullOrWhiteSpace(filter.PetName))
        {
            sqlBuilder.AppendLine($" AND LOWER(p.{PetsTable.Name}) LIKE LOWER(@PetName)");

            if (parameters.ParameterNames.Contains("PetName") == false)
                parameters.Add("PetName", $"%{filter.PetName}%");
        }

        if (!string.IsNullOrWhiteSpace(filter.Color))
        {
            sqlBuilder.AppendLine($" AND LOWER(p.{PetsTable.Color}) LIKE LOWER(@Color)");

            if (parameters.ParameterNames.Contains("Color") == false)
                parameters.Add("Color", $"%{filter.Color}%");
        }

        if (!string.IsNullOrWhiteSpace(filter.City))
        {
            sqlBuilder.AppendLine($" AND LOWER(p.{PetsTable.AddressCity}) LIKE LOWER(@City)");
            parameters.Add("City", $"%{filter.City}%");
        }

        if (filter.SpeciesIds != null && filter.SpeciesIds.Any()
            || filter.BreedIds != null && filter.BreedIds.Any())
        {
            sqlBuilder.AppendLine(" AND (");
            string or = string.Empty;
            if (filter.SpeciesIds != null && filter.SpeciesIds.Any())
            {
                sqlBuilder.AppendLine($"p.{PetsTable.PetTypeSpeciesId} = ANY(@SpeciesIds)");
                parameters.Add($"SpeciesIds", filter.SpeciesIds);
                or = " OR ";
            }

            if (filter.BreedIds != null && filter.BreedIds.Any())
            {
                sqlBuilder.AppendLine($" {or} p.{PetsTable.PetTypeBreedId} = ANY(@BreedIds)");
                parameters.Add($"BreedIds", filter.BreedIds);
            }
            sqlBuilder.AppendLine(" )");
        }

        if (filter.MinAgeInMonth > 0)
        {
            var currentDate = DateTime.UtcNow;
            var maxDateOfBirthday = currentDate.AddMonths(-filter.MinAgeInMonth);
            sqlBuilder.AppendLine($" AND p.{PetsTable.DateOfBirth} <= @MaxDate");

            if (parameters.ParameterNames.Contains("MaxDate") == false)
                parameters.Add("MaxDate", maxDateOfBirthday);
        }

        if (filter.MaxAgeInMonth > 0)
        {
            var currentDate = DateTime.UtcNow;
            var minDateOfBirthday = currentDate.AddMonths(-filter.MaxAgeInMonth);
            sqlBuilder.AppendLine($" AND p.{PetsTable.DateOfBirth} >= @MinDate");

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
            sql.AppendLine($" ORDER BY p.{PetsTable.Id}");
            return sql;
        }
        var orderClauses = orderBies.Select(o =>
        {
            var direction = o.OrderDirection == OrderDirection.Desc ? "DESC" : "ASC";
            var column = o.OrderByProperty.ToLower() switch
            {
                "name" => $"p.{PetsTable.Name} {direction}",

                "age" => $"p.{PetsTable.DateOfBirth} {direction}",

                "volunteer" => $"v.{VolunteersTable.LastName} {direction}," +
                               $"v.{VolunteersTable.FirstName} {direction}",

                "species" => $"s.{SpeciesTable.Name} {direction}, " +
                           $"b.{BreedsTable.Name} {direction}",

                "status" => $"p.{PetsTable.HelpStatus} {direction}",

                "address" => $"p.{PetsTable.AddressCity} {direction}, " +
                           $"p.{PetsTable.AddressRegion} {direction}," +
                           $"p.{PetsTable.AddressStreet} {direction}",

                _ => $"p.{PetsTable.Id} {direction}"
            };
            return $"{column}";
        });

        sql.AppendLine(" ORDER BY " + string.Join(", ", orderClauses));

        return sql;
    }
}
