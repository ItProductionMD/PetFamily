using Microsoft.Extensions.Logging;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.Auth.Public.Contracts;
using PetFamily.Discussions.Application.Dtos;
using PetFamily.Discussions.Application.IRepositories;
using PetFamily.SharedApplication.IUserContext;
using PetFamily.SharedApplication.PaginationUtils;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;

namespace PetFamily.Discussions.Application.Queries.GetDiscussion;

public class GetDiscussionHandler (
    IDiscussionReadRepository discussionReadRepo,
    ILogger<GetDiscussionHandler> logger,
    IParticipantContract participantContract): IQueryHandler<GetDiscussionResponse, GetDiscussionQuery>
{
    private readonly IDiscussionReadRepository _discussionReadRepo = discussionReadRepo;
    private readonly ILogger<GetDiscussionHandler> _logger = logger;
    private readonly IParticipantContract _participantContract = participantContract;

    public async Task<Result<GetDiscussionResponse>> Handle(GetDiscussionQuery cmd, CancellationToken ct)
    {
        var contextUserId = cmd.UserId;

        var paginationParams = new PaginationParams (cmd.Page, cmd.PageSize);
        
        var getDiscussionDto = await _discussionReadRepo.GetById(cmd.DiscussionId, paginationParams, ct);
        if(getDiscussionDto.IsFailure)
            return Result.Fail(getDiscussionDto.Error);
        
        var discussionDto = getDiscussionDto.Data!;
        if(discussionDto.ParticipantIds.Contains(contextUserId) == false)
        {
            _logger.LogError("Authorization Forbidden. User with id: {contextUserId} does not have" +
                " the access  to discussion with id: {DiscussionId}!", contextUserId, cmd.DiscussionId);

            return Result.Fail(Error.Forbidden($"User with Id:{contextUserId} does not have" +
                $" access to discussion with id:{cmd.DiscussionId} "));
        }
        try
        {
            var getParticipantDtos = await _participantContract.GetByIds(discussionDto.ParticipantIds, ct);
            if(getParticipantDtos.IsFailure)
                return Result.Fail(Error.InternalServerError("Error while getting info about participants "));
            
            _logger.LogInformation("User with id: {contextUserId} has got discussion with Id:{DiscussionId} successful",
            contextUserId, cmd.DiscussionId);

            var response = new GetDiscussionResponse(getParticipantDtos.Data!, discussionDto);

            return Result.Ok(response);

        }
        catch(Exception ex)
        {
            _logger.LogCritical("Exception while getting info about participants! Error:{error}",
                    ex.Message);

            return Result.Fail(Error.InternalServerError("Error while getting info about participants "));
        }   
    }
}
