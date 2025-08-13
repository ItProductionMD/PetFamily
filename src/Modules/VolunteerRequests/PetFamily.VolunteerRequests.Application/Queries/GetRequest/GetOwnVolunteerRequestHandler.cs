using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Results;
using PetFamily.VolunteerRequests.Application.Dtos;
using PetFamily.VolunteerRequests.Application.IRepositories;

namespace PetFamily.VolunteerRequests.Application.Queries.GetRequest;

public class GetOwnVolunteerRequestHandler(IVolunteerRequestReadRepository volunteerRequestReadRepo)
    : IQueryHandler<VolunteerRequestDto?, GetOwnVolunteerRequestQuery>
{
    public async Task<Result<VolunteerRequestDto>> Handle(
        GetOwnVolunteerRequestQuery query,
        CancellationToken ct)
    {
        var userId = query.UserId;
        return (await volunteerRequestReadRepo.GetByUserIdAsync(userId, ct));
    }
}
