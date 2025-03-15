using System.Data;
using Microsoft.Extensions.Configuration;
using System.Data.Common;
using PetFamily.Application.Dtos;
using PetFamily.Application.Queries;
using Dapper;
using PetFamily.Application.IRepositories;

namespace PetFamily.Infrastructure.Repositories.Read;

public class VolunteerReadRepositoryWithDapper(IDbConnection dbConnection) : IVolunteerReadRepository
{
    private readonly IDbConnection _dbConnection = dbConnection;

    public async Task<GetVolunteersResponse> GetVolunteers(
     GetVolunteersQuery query,
     CancellationToken cancelToken = default)
    {
        var orderBy = query.orderBy switch
        {
            "full_name" => "v.last_name, v.first_name",
            "rating" => "v.rating",
            _ => "v.Id"
        };
        var orderDirection = query.orderDirection switch
        {
            "asc" => "asc",
            _ => "desc"
        };

        var offset = (query.pageNumber - 1) * query.pageSize;
        var limit = query.pageSize + 1;

        var volunteersList = await _dbConnection.QueryAsync<VolunteerMainInfoDto>(
            $@"
                SELECT v.Id, 
                CONCAT(v.last_name, ' ', v.first_name) AS FullName, 
                CONCAT(v.phone_region_code, '-', v.phone_number) AS Phone, 
                v.rating
                FROM Volunteers v
                ORDER BY {orderBy} {orderDirection}
                LIMIT @Limit OFFSET @Offset",
                new { Limit = limit, Offset = offset });

        var volunteersListAsList = volunteersList.ToList();
        bool hasMoreRecords = volunteersListAsList.Count > query.pageSize;

        if (hasMoreRecords)
            volunteersListAsList.RemoveAt(volunteersListAsList.Count - 1);

        var totalCount = hasMoreRecords
            ? await _dbConnection.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM Volunteers")
            : (query.pageNumber - 1) * query.pageSize + volunteersListAsList.Count;

        return new GetVolunteersResponse(totalCount, volunteersListAsList);
    }

}
