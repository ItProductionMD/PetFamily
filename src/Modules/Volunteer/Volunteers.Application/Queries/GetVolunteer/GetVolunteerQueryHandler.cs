using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Results;
using Volunteers.Application.IRepositories;
using Volunteers.Application.Queries.GetVolunteers;
using Volunteers.Application.ResponseDtos;

namespace Volunteers.Application.Queries.GetVolunteer;

public class GetVolunteerQueryHandler(
    IVolunteerReadRepository repository,
    ILogger<GetVolunteersQueryHandler> logger)
    : IQueryHandler<VolunteerDto, GetVolunteerQuery>
{
    private readonly ILogger _logger = logger;
    private readonly IVolunteerReadRepository _repository = repository;

    public async Task<Result<VolunteerDto>> Handle(GetVolunteerQuery query, CancellationToken ct) =>
          await _repository.GetByIdAsync(query.Id, ct);
}
